using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.Core.Abstractions.Components;
using WinformsBoilerplate.Core.Abstractions.Services;

namespace WinformsBoilerplate.Infrastructure.Services;

public class LayoutService(IServiceProvider sp) : ILayoutService
{
    private readonly Dictionary<string, IOverlayControl?> _overlayControls = [];

    /// <inheritdoc />
    public void HideOverlay(Control ctrl, bool dispose = true)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? overlayControl);

        if (overlayControl is null)
        {
            return;
        }

        overlayControl.HideOverlay(dispose);

        if (dispose)
        {
            _overlayControls[ctrl.Name] = null;
        }
    }

    /// <inheritdoc />
    public bool IsOverlayVisible(string ctrlName)
    {
        _ = _overlayControls.TryGetValue(ctrlName, out IOverlayControl? overlayControl);

        return overlayControl is not null && overlayControl.Visible;
    }

    /// <inheritdoc />
    public void ShowOverlay(Control ctrl, string overlayText)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? value);

        IOverlayControl overlayControl = value ?? sp.GetRequiredService<IOverlayControl>();
        overlayControl.OverlayLabelText = overlayText;
        overlayControl.ShowOverlay(ctrl);

        if (!_overlayControls.TryAdd(ctrl.Name, overlayControl))
        {
            _overlayControls[ctrl.Name] = overlayControl;
        }
    }

    /// <inheritdoc />
    public void ToggleOverlay(Control ctrl, string overlayText, bool state)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? value);

        if (state)
        {
            if (value is not null)
            {
                value.OverlayLabelText = overlayText;
                value.ShowOverlay(ctrl);
            }
            else
            {
                IOverlayControl overlayControl = sp.GetRequiredService<IOverlayControl>();
                overlayControl.OverlayLabelText = overlayText;
                overlayControl.ShowOverlay(ctrl);

                if (!_overlayControls.TryAdd(ctrl.Name, overlayControl))
                {
                    _overlayControls[ctrl.Name] = overlayControl;
                }
            }

            return;
        }

        if (value is not null)
        {
            value.HideOverlay();
            _overlayControls[ctrl.Name] = null;
        }
    }

    /// <inheritdoc />
    public void UpdateOverlayText(Control ctrl, string newText)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? value);

        if (value is not null)
        {
            value.OverlayLabelText = newText;
        }
    }
}
