using System.ComponentModel;
using WinformsBoilerplate.App.Shared.Controls;
using WinformsBoilerplate.Core.Abstractions.Components.Controls;
using WinformsBoilerplate.Core.Helpers;

namespace WinformsBoilerplate.App.Components.Controls;

public class OverlayControl : StandardControlBase, IOverlayControl
{
    private readonly Label _label;

    private Bitmap? blurredBackground;

    public OverlayControl()
    {
        SetStyle(
            ControlStyles.SupportsTransparentBackColor |
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true
        );

        BackColor = Color.Transparent;
        Dock = DockStyle.Fill;
        Visible = false;

        _label = new() {
            ForeColor = SystemColors.ControlText,
            AutoSize = true,
            Font = new Font("Inter", 12F, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        Controls.Add(_label);
        CenterLoadingLabel();

        Resize += (s, e) => CenterLoadingLabel();
    }

    /// <inheritdoc cref="StandardControlBase.DisposeUnmanagedResources" />
    public override void DisposeUnmanagedResources()
    {
        blurredBackground?.Dispose();
        blurredBackground = null;
    }

    /// <inheritdoc cref="IOverlayControl.OverlayText" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string OverlayText { get; set; } = string.Empty;

    /// <inheritdoc cref="IOverlayControl.ShowOverlay(Control)" />
    public void ShowOverlay(Control parentCtrl)
    {
        _label.Text = OverlayText;

        if (Parent != parentCtrl)
        {
            parentCtrl.Controls.Add(this);
        }

        BringToFront();
        CaptureAndBlurBackground(parentCtrl);
        Visible = true;
        Invalidate();
    }

    /// <inheritdoc cref="IOverlayControl.HideOverlay(bool)" />
    public void HideOverlay(bool disposing = false)
    {
        Visible = false;
        blurredBackground?.Dispose();
        blurredBackground = null;

        if (disposing)
        {
            Dispose();
        }

    }

    /// <inheritdoc cref="Control.OnPaintBackground(PaintEventArgs)"/>
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        if (blurredBackground != null)
        {
            pevent.Graphics.DrawImage(blurredBackground, 0, 0, Width, Height);
        }
    }

    /// <summary>
    /// Centers the loading label within the overlay control.
    /// </summary>
    private void CenterLoadingLabel()
    {
        if (_label != null)
        {
            _label.Location = new Point(
                (Width - _label.Width) / 2,
                (Height - _label.Height) / 2
            );
        }
    }

    /// <summary>
    /// Captures the background of the specified parent control and applies a blur effect.
    /// </summary>
    /// <param name="parent"></param>
    private void CaptureAndBlurBackground(Control parent)
    {
        int blurRadius = 8;
        int padding = blurRadius * 2;

        if (blurredBackground != null)
        {
            blurredBackground.Dispose();
            blurredBackground = null;
        }

        // Capture the exact size first
        Bitmap original = new(parent.Width, parent.Height);
        parent.DrawToBitmap(original, new Rectangle(0, 0, parent.Width, parent.Height));

        // Create a larger bitmap to avoid edge artifacts
        Bitmap padded = new(parent.Width + padding, parent.Height + padding);
        using var paddedGraphic = Graphics.FromImage(padded);
        // Fill center
        paddedGraphic.DrawImage(
            image: original,
            x: padding / 2,
            y: padding / 2,
            width: parent.Width,
            height: parent.Height
        );

        // Stretch edges to fill padding
        // Top
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(padding / 2, 0, parent.Width, padding / 2),
            srcRect: new Rectangle(0, 0, parent.Width, 1),
            srcUnit: GraphicsUnit.Pixel
        );

        // Bottom
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(padding / 2, parent.Height + padding / 2, parent.Width, padding / 2),
            srcRect: new Rectangle(0, parent.Height - 1, parent.Width, 1),
            srcUnit: GraphicsUnit.Pixel
        );

        // Left
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(0, padding / 2, padding / 2, parent.Height),
            srcRect: new Rectangle(0, 0, 1, parent.Height),
            srcUnit: GraphicsUnit.Pixel
        );

        // Right
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(parent.Width + padding / 2, padding / 2, padding / 2, parent.Height),
            srcRect: new Rectangle(parent.Width - 1, 0, 1, parent.Height),
            srcUnit: GraphicsUnit.Pixel
        );

        // Corners (copy corner pixels)
        // Top-left
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(0, 0, padding / 2, padding / 2),
            srcRect: new Rectangle(0, 0, 1, 1),
            srcUnit: GraphicsUnit.Pixel
        );

        // Top-right
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(parent.Width + padding / 2, 0, padding / 2, padding / 2),
            srcRect: new Rectangle(parent.Width - 1, 0, 1, 1),
            srcUnit: GraphicsUnit.Pixel
        );

        // Bottom-left
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(0, parent.Height + padding / 2, padding / 2, padding / 2),
            srcRect: new Rectangle(0, parent.Height - 1, 1, 1),
            srcUnit: GraphicsUnit.Pixel
        );

        // Bottom-right
        paddedGraphic.DrawImage(
            image: original,
            destRect: new Rectangle(parent.Width + padding / 2, parent.Height + padding / 2, padding / 2, padding / 2),
            srcRect: new Rectangle(parent.Width - 1, parent.Height - 1, 1, 1),
            srcUnit: GraphicsUnit.Pixel
        );

        original.Dispose();

        Bitmap blurred = ControlHelpers.GaussianBlur(padded, blurRadius, 2.0);
        padded.Dispose();

        blurredBackground = new Bitmap(parent.Width, parent.Height);
        using var blurredGraphic = Graphics.FromImage(blurredBackground);
        blurredGraphic.DrawImage(
            image: blurred,
            destRect: new Rectangle(0, 0, blurredBackground.Width, blurredBackground.Height),
            srcRect: new Rectangle(padding / 2, padding / 2, blurredBackground.Width, blurredBackground.Height),
            srcUnit: GraphicsUnit.Pixel
        );

        blurred.Dispose();
    }

}
