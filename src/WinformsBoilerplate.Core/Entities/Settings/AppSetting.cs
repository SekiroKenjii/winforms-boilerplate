using WinformsBoilerplate.Core.Helpers;

namespace WinformsBoilerplate.Core.Entities.Settings;

/// <summary>
/// Represents the application settings.
/// </summary>
public sealed class AppSetting : IEquatable<AppSetting>
{
    /// <summary>
    /// Gets or sets the miscellaneous settings for the application.
    /// </summary>
    public MiscSetting Misc { get; set; } = new();

    public bool Equals(AppSetting? other)
    {
        return other is not null && ObjectHelpers.Compare(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is AppSetting other && ObjectHelpers.Compare(this, other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Misc);
    }
}
