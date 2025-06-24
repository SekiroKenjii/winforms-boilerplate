using System.ComponentModel;
using WinformsBoilerplate.Core.Entities.Contracts;
using WinformsBoilerplate.Core.Enums;

namespace WinformsBoilerplate.Core.Entities.Settings;

/// <summary>
/// Represents the application settings.
/// </summary>
/// <remarks>This class provides a container for application-wide settings, with support for observing changes to
/// nested properties. Any changes to its properties are automatically tracked to update the state of this entity.</remarks>
public class AppSettings : ObservableEntityBase
{
    private MiscSettings _misc = new();

    public AppSettings()
    {
        _misc.PropertyChanged += NestedPropertyChanged;
    }

    /// <summary>
    /// Gets or sets the miscellaneous settings for the application.
    /// </summary>
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

    /// <inheritdoc cref="ObservableEntityBase.AfterInit" />
    public override void AfterInit()
    {
        base.AfterInit();

        Misc.AfterInit();
    }

    /// <inheritdoc cref="ObservableEntityBase.BeginUpdate" />
    public override void BeginUpdate()
    {
        base.BeginUpdate();

        Misc.BeginUpdate();
    }

    /// <inheritdoc cref="ObservableEntityBase.AfterSaveChanges" />
    public override void AfterSaveChanges()
    {
        base.AfterSaveChanges();

        Misc.AfterSaveChanges();
    }
}
