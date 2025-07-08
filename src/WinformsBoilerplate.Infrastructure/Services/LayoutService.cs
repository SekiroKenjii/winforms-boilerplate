using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Components.Controls;
using WinformsBoilerplate.Core.Abstractions.Services;

namespace WinformsBoilerplate.Infrastructure.Services;

public class LayoutService(IServiceProvider sp) : Disposable, ILayoutService
{
    private readonly Dictionary<string, IOverlayControl?> _overlayControls = [];

    /// <inheritdoc cref="ILayoutService.HideOverlay(Control, bool)" />
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

    /// <inheritdoc cref="ILayoutService.IsOverlayVisible(string)" />
    public bool IsOverlayVisible(string ctrlName)
    {
        _ = _overlayControls.TryGetValue(ctrlName, out IOverlayControl? overlayControl);

        return overlayControl is not null && overlayControl.Visible;
    }

    /// <inheritdoc cref="ILayoutService.ShowOverlay(Control, string)" />
    public void ShowOverlay(Control ctrl, string overlayText)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? value);

        IOverlayControl overlayControl = value ?? sp.GetRequiredService<IOverlayControl>();
        overlayControl.OverlayText = overlayText;
        overlayControl.ShowOverlay(ctrl);

        if (!_overlayControls.TryAdd(ctrl.Name, overlayControl))
        {
            _overlayControls[ctrl.Name] = overlayControl;
        }
    }

    /// <inheritdoc cref="ILayoutService.ToggleOverlay(Control, string, bool)" />
    public void ToggleOverlay(Control ctrl, string overlayText, bool state)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? value);

        if (state)
        {
            if (value is not null)
            {
                value.OverlayText = overlayText;
                value.ShowOverlay(ctrl);
            }
            else
            {
                IOverlayControl overlayControl = sp.GetRequiredService<IOverlayControl>();
                overlayControl.OverlayText = overlayText;
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

    /// <inheritdoc cref="ILayoutService.UpdateOverlayText(Control, string)" />
    public void UpdateOverlayText(Control ctrl, string newText)
    {
        _ = _overlayControls.TryGetValue(ctrl.Name, out IOverlayControl? value);

        if (value is not null)
        {
            value.OverlayText = newText;
        }
    }

    /// <inheritdoc cref="Disposable.Dispose(bool)" />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (Disposed)
        {
            return;
        }

        foreach (IOverlayControl? overlayControl in _overlayControls.Values)
        {
            overlayControl?.Dispose();
        }

        _overlayControls.Clear();
    }
}
