using System.ComponentModel;
using WinformsBoilerplate.Core.Entities.Contracts;
using WinformsBoilerplate.Core.Enums;

namespace WinformsBoilerplate.Core.Entities.Settings;

public class AppSettings : ObservableEntityBase
{
    private MiscSettings _misc = new();

    public AppSettings()
    {
        // Subscribe to property changes in nested objects
        _misc.PropertyChanged += NestedPropertyChanged;
    }

    public MiscSettings Misc
    {
        get => _misc;
        set {
            // Unsubscribe from old instance's property changes
            _misc.PropertyChanged -= NestedPropertyChanged;

            // Since we initialize _misc with a new instance and never set it to null,
            // we can ensure it will never be null here
            if (SetProperty(ref _misc, value ?? new MiscSettings()))
            {
                // Subscribe to new instance's property changes
                _misc.PropertyChanged += NestedPropertyChanged;
            }
        }
    }

    /// <summary>
    /// Event handler for property changes in nested objects.
    /// Updates the state of this entity when a nested entity changes.
    /// </summary>
    private void NestedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Don't propagate state change events to avoid circular updates
        if (e.PropertyName == nameof(State))
        {
            return;
        }

        // Only update state if we're not initializing
        if (State != ModelState.Init)
        {
            State = ModelState.Modified;
        }
    }

    /// <inheritdoc />
    public override void AfterInit()
    {
        base.AfterInit();
        Misc.AfterInit();
    }

    /// <inheritdoc />
    public override void BeginUpdate()
    {
        base.BeginUpdate();
        Misc.BeginUpdate();
    }

    /// <inheritdoc />
    public override void AfterSaveChanges()
    {
        base.AfterSaveChanges();
        Misc.AfterSaveChanges();
    }
}
