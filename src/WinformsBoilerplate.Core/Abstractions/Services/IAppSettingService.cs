using WinformsBoilerplate.Core.Entities.Settings;

namespace WinformsBoilerplate.Core.Abstractions.Services;

/// <summary>
/// Provides methods for managing application settings, including retrieving, saving, and creating default settings
/// files.
/// </summary>
/// <remarks>
/// This service is designed to handle application settings in a consistent and centralized manner.
/// Implementations of this interface should ensure thread safety and proper handling of storage operations.
/// </remarks>
public interface IAppSettingService : ISingletonDependency
{
    /// <summary>
    /// Gets a value indicating whether the setting's state has been modified since it was last saved or initialized.
    /// </summary>
    bool IsChanged { get; }

    /// <summary>
    /// Gets the application setting associated with this instance.
    /// </summary>
    AppSetting Value { get; }

    /// <summary>
    /// Gets the most recently retrieved application setting.
    /// </summary>
    AppSetting LastValue { get; }

    /// <summary>
    /// Creates a default settings file in the application's configuration directory.
    /// </summary>
    /// <remarks>
    /// If the settings file already exists and <paramref name="override"/> is <see
    /// langword="false"/>, the method does not modify the existing file. Ensure the application has appropriate
    /// permissions to write to the configuration directory.
    /// </remarks>
    /// <param name="override">A value indicating whether to overwrite the existing settings file if it already exists. <see langword="true"/>
    /// to overwrite the file; otherwise, <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if the default settings file was successfully created or already exists; otherwise, <see langword="false"/>.</returns>
    bool CreateDefaultSettingFile(bool @override = false);

    /// <summary>
    /// Saves the specified application setting to the underlying storage.
    /// </summary>
    /// <remarks>This method persists the provided <see cref="AppSetting"/> instance to the configured storage
    /// medium. If an existing setting with the same key exists, it will be overwritten.</remarks>
    /// <param name="appSetting">The application setting to save. Cannot be null.</param>
    void Save(AppSetting appSetting);
}
