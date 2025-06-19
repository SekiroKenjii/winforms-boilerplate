using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinformsBoilerplate.Core.Enums;

namespace WinformsBoilerplate.Core.Entities.Contracts;

/// <summary>
/// Base class for entities that implement the IObservableEntity interface.
/// Provides automatic property change tracking and state management.
/// </summary>
public abstract class ObservableEntityBase : IObservableEntity, INotifyPropertyChanged
{
    private ModelState _state = ModelState.Init;

    /// <summary>
    /// Event that is triggered when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public ModelState State
    {
        get => _state;
        protected set => SetProperty(ref _state, value);
    }

    /// <inheritdoc />
    public virtual void AfterInit()
    {
        State = ModelState.None;
    }

    /// <inheritdoc />
    public virtual void BeginUpdate()
    {
        State = ModelState.Modified;
    }

    /// <inheritdoc />
    public virtual void AfterSaveChanges()
    {
        State = ModelState.None;
    }

    /// <summary>
    /// Sets a property value and raises the PropertyChanged event.
    /// Also updates the State to Modified when a property changes.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The name of the property (auto-detected).</param>
    /// <returns>True if the value was changed, false otherwise.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // If value is the same, do nothing
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;

        // Only update state if we're not currently initializing
        if (State != ModelState.Init)
        {
            State = ModelState.Modified;
        }

        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}