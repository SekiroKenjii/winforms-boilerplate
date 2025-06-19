using WinformsBoilerplate.Core.Enums;

namespace WinformsBoilerplate.Core.Entities.Contracts;

/// <summary>
/// Represents an observable entity that can be updated and notified of changes.
/// </summary>
public interface IObservableEntity
{
    /// <summary>
    /// Gets the current state of the observable entity.
    /// </summary>
    ModelState State { get; }

    /// <summary>
    /// Performs post-initialization tasks required to finalize the setup of the object.
    /// </summary>
    /// <remarks>
    /// This method should be called after the object has been initialized to ensure that  any
    /// additional setup or configuration is completed. Failure to call this method  may result in the object being in
    /// an incomplete or unusable state.
    /// </remarks>
    void AfterInit();

    /// <summary>
    /// Prepares the observable entity for updates, typically setting its state to Modified.
    /// </summary>
    /// <remarks>
    /// This method indicates that the entity is now in a state ready to receive updates.
    /// </remarks>
    void BeginUpdate();

    /// <summary>
    /// Finalizes any changes made to the observable entity and resets its state to None.
    /// </summary>
    /// <remarks>
    /// This method should be called after all updates have been applied to ensure that the entity is in a stable state.
    /// </remarks>
    void AfterSaveChanges();
}
