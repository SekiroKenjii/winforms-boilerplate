using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using WinformsBoilerplate.Core.Structs.Win32.Security.Credentials;
using WinformsBoilerplate.Core.Win32.API;

namespace WinformsBoilerplate.Core.Helpers;

public static class ControlHelpers
{
    /// <summary>
    /// Applies a true Gaussian blur to the provided bitmap.
    /// </summary>
    /// <param name="image">The bitmap image to blur.</param>
    /// <param name="radius">The blur radius (kernel size will be 2 * radius + 1).</param>
    /// <param name="sigma">Standard deviation for the Gaussian distribution. If zero, it's estimated from radius.</param>
    /// <returns>A new blurred bitmap.</returns>
    public static Bitmap GaussianBlur(Bitmap image, int radius, double sigma)
    {
        if (radius < 1)
        {
            return image;
        }

        Bitmap blurred = new(image.Width, image.Height);
        Rectangle rect = new(0, 0, image.Width, image.Height);

        BitmapData srcData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        BitmapData dstData = blurred.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        try
        {
            int bufferSize = srcData.Stride * srcData.Height;
            byte[] sourceBuffer = new byte[bufferSize];
            byte[] tempBuffer = new byte[bufferSize];
            byte[] resultBuffer = new byte[bufferSize];

            Marshal.Copy(srcData.Scan0, sourceBuffer, 0, bufferSize);

            // Estimate sigma if not provided
            if (sigma <= 0)
            {
                sigma = radius / 2.0;
            }

            double[] kernel = CreateGaussianKernel(radius, sigma);

            // Horizontal pass
            Convolve(sourceBuffer, tempBuffer, image.Width, image.Height, srcData.Stride, kernel, horizontal: true);
            // Vertical pass
            Convolve(tempBuffer, resultBuffer, image.Width, image.Height, srcData.Stride, kernel, horizontal: false);

            Marshal.Copy(resultBuffer, 0, dstData.Scan0, bufferSize);
        }
        finally
        {
            image.UnlockBits(srcData);
            blurred.UnlockBits(dstData);
        }

        return blurred;
    }

    /// <summary>
    /// Creates a 1D Gaussian kernel for use in convolution.
    /// </summary>
    private static double[] CreateGaussianKernel(int radius, double sigma)
    {
        int length = (radius * 2) + 1;
        double[] kernel = new double[length];
        double sum = 0;
        int index = 0;

        for (int i = -radius; i <= radius; i++)
        {
            double value = Math.Exp(-(i * i) / (2 * sigma * sigma));
            kernel[index++] = value;
            sum += value;
        }

        // Normalize
        for (int i = 0; i < kernel.Length; i++)
        {
            kernel[i] /= sum;
        }

        return kernel;
    }

    /// <summary>
    /// Applies 1D convolution to the buffer (horizontal or vertical).
    /// </summary>
    private static void Convolve(byte[] source, byte[] output, int width, int height, int stride, double[] kernel, bool horizontal)
    {
        int radius = kernel.Length / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double r = 0, g = 0, b = 0, a = 0;

                for (int k = -radius; k <= radius; k++)
                {
                    int offsetX = horizontal ? Clamp(x + k, 0, width - 1) : x;
                    int offsetY = horizontal ? y : Clamp(y + k, 0, height - 1);

                    int srcOffset = (offsetY * stride) + (offsetX * 4);
                    double weight = kernel[k + radius];

                    b += source[srcOffset] * weight;
                    g += source[srcOffset + 1] * weight;
                    r += source[srcOffset + 2] * weight;
                    a += source[srcOffset + 3] * weight;
                }

                int dstOffset = (y * stride) + (x * 4);
                output[dstOffset] = (byte)b;
                output[dstOffset + 1] = (byte)g;
                output[dstOffset + 2] = (byte)r;
                output[dstOffset + 3] = (byte)a;
            }
        }
    }

    /// <summary>
    /// Ensures a value is within a specified min/max range.
    /// </summary>
    private static int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    /// <summary>
    /// Displays a Windows authentication dialog and validates user credentials.
    /// </summary>
    /// <param name="hWnd">Handle to the parent window.</param>
    /// <returns><c>true</c> if authentication succeeded; otherwise, <c>false</c>.</returns>
    public static bool ShowWindowsAuthDialog(IntPtr hWnd)
    {
        CredUIInfoW credUI = new() {
            HwndParent = (HWND)hWnd,
            PszMessageText = "Please enter your Windows credentials.",
            PszCaptionText = "Authentication Required",
        };

        _ = CredUI.CredUIPromptForWindowsCredentials(credUI, out IntPtr outCredBuffer, out uint outCredSize);

        (string username, string domain, string password) = CredUI.CredUnPackAuthenticationBuffer(outCredBuffer, outCredSize);

        return AdvApi32.LogonUser(username, domain, password);
    }
}
