using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace WinformsBoilerplate.App.Shared;

/// <summary>
/// Represents a base container that provides high DPI support for Windows Forms applications.
/// </summary>
/// <remarks>
/// This class extends the <see cref="Form"/> class to include functionality for adjusting controls and menu items
/// to high DPI settings. It provides methods to adjust the size and appearance of controls and images based on the
/// current DPI settings.
/// </remarks>
public partial class HiDpiContainerBase : Form
{
    private float _sDpiRatio = 1.0F;

    public HiDpiContainerBase()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Adjusts the controls within the specified control collections to account for high DPI settings.
    /// </summary>
    /// <param name="collections">A span of <see cref="Control.ControlCollection"/> objects to adjust for high DPI.</param>
    /// <returns>An enumerable collection of <see cref="IDisposable"/> objects representing the adjusted controls.</returns>
    protected IEnumerable<IDisposable> AdjustControlsThroughDpi(params Span<Control.ControlCollection> collections)
    {
        if (DpiRatioIsOne())
        {
            return [];
        }

        List<IDisposable> list = [];

        Array.ForEach(collections.ToArray(), controlCollection => {
            foreach (Control control in controlCollection)
            {
                switch (control)
                {
                    case null:
                        continue;
                    case ButtonBase button:
                    {
                        Image? image = button.Image;

                        if (image == null)
                        {
                            continue;
                        }

                        Bitmap imageStretched = GetImageStretchedDpi(image);
                        button.Image = imageStretched;
                        list.Add(imageStretched);

                        continue;
                    }
                }

                Control.ControlCollection nestedControls = control.Controls;

                if (nestedControls is { Count: 0 })
                {
                    continue;
                }

                list.AddRange(AdjustControlsThroughDpi(nestedControls));
            }
        });

        return list;
    }

    /// <summary>
    /// Adjusts the specified controls to account for high DPI settings.
    /// </summary>
    /// <param name="controls">A span of <see cref="Control"/> objects to adjust for high DPI.</param>
    /// <returns>An enumerable collection of <see cref="IDisposable"/> objects representing the adjusted controls.</returns>
    protected IEnumerable<IDisposable> AdjustControlsThroughDpi(params Span<Control> controls)
    {
        if (DpiRatioIsOne())
        {
            return [];
        }

        List<IDisposable> list = [];

        Array.ForEach(controls.ToArray(), control => {
            if (control is not ButtonBase button)
            {
                return;
            }

            Image? image = button.Image;

            if (image == null)
            {
                return;
            }

            Bitmap imageStretched = GetImageStretchedDpi(image);
            button.Image = imageStretched;
            list.Add(imageStretched);
        });

        return list;
    }

    /// <summary>
    /// Adjusts the menu strip items within the specified collections to account for high DPI settings.
    /// </summary>
    /// <param name="collections">A span of <see cref="ToolStripItemCollection"/> objects to adjust for high DPI.</param>
    /// <returns>An enumerable collection of <see cref="IDisposable"/> objects representing the adjusted menu items.</returns>
    protected IEnumerable<IDisposable> AdjustMenuStripItemsThroughDpi(params Span<ToolStripItemCollection> collections)
    {
        if (DpiRatioIsOne())
        {
            return [];
        }

        List<IDisposable> list = [];

        Array.ForEach(collections.ToArray(), collection => {
            list.AddRange(ProcessToolStripItemCollection(collection));
        });

        return list;
    }

    /// <summary>
    /// Processes a ToolStripItemCollection to adjust images for high DPI.
    /// </summary>
    /// <param name="collection">The collection to process.</param>
    /// <returns>A list of disposable resources created during processing.</returns>
    private IEnumerable<IDisposable> ProcessToolStripItemCollection(ToolStripItemCollection collection)
    {
        List<IDisposable> list = [];

        foreach (object? component in collection)
        {
            if (component is not ToolStripMenuItem rootMenuItem)
            {
                continue;
            }

            list.AddRange(ProcessMenuItemDropDowns(rootMenuItem.DropDownItems));
        }

        return list;
    }

    /// <summary>
    /// Processes the drop-down items of a menu item to adjust images for high DPI.
    /// </summary>
    /// <param name="dropDownItems">The drop-down items to process.</param>
    /// <returns>A list of disposable resources created during processing.</returns>
    private IEnumerable<IDisposable> ProcessMenuItemDropDowns(ToolStripItemCollection dropDownItems)
    {
        List<IDisposable> list = [];

        foreach (object? subComponent in dropDownItems)
        {
            if (subComponent is not ToolStripMenuItem subMenuItem)
            {
                continue;
            }

            if (subMenuItem.Image != null)
            {
                Bitmap imageStretched = GetImageStretchedDpi(subMenuItem.Image);
                subMenuItem.Image = imageStretched;
                list.Add(imageStretched);
            }

            if (subMenuItem.DropDownItems is { Count: > 0 })
            {
                list.AddRange(AdjustMenuStripItemsThroughDpi(subMenuItem.DropDownItems));
            }
        }

        return list;
    }

    /// <summary>
    /// Determines whether the current DPI ratio is 1 (i.e., no scaling).
    /// </summary>
    /// <returns><c>true</c> if the DPI ratio is 1; otherwise, <c>false</c>.</returns>
    protected bool DpiRatioIsOne()
    {
        Graphics graphics = CreateGraphics();
        float dpiX = graphics.DpiX;
        _sDpiRatio = dpiX / 96F;

        return Math.Abs(_sDpiRatio - 1) <= 0;
    }


    /// <summary>
    /// Stretches the specified image to account for high DPI settings.
    /// </summary>
    /// <param name="imageIn">The <see cref="Image"/> to stretch.</param>
    /// <returns>A <see cref="Bitmap"/> representing the stretched image.</returns>
    private Bitmap GetImageStretchedDpi(Image imageIn)
    {
        Debug.Assert(imageIn != null);

        int newWidth = (int)Math.Round(imageIn.Width * _sDpiRatio);
        int newHeight = (int)Math.Round(imageIn.Height * _sDpiRatio);
        Bitmap newBitmap = new(newWidth, newHeight);

        using var graphics = Graphics.FromImage(newBitmap);

        // According to this blog post http://blogs.msdn.com/b/visualstudio/archive/2014/03/19/improving-high-dpi-support-for-visual-studio-2013.aspx
        // NearestNeighbor is more adapted for 200% and 200%+ DPI
        InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;

        if (_sDpiRatio >= 2.0f)
        {
            interpolationMode = InterpolationMode.NearestNeighbor;
        }

        graphics.InterpolationMode = interpolationMode;
        graphics.DrawImage(imageIn, new Rectangle(0, 0, newWidth, newHeight));
        imageIn.Dispose();

        return newBitmap;
    }
}
