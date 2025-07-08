using KellermanSoftware.CompareNetObjects;

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
        if (other is null)
        {
            return false;
        }

        var compareLogic = new CompareLogic();
        ComparisonResult result = compareLogic.Compare(this, other);

        return result.AreEqual;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not AppSetting other)
        {
            return false;
        }

        var compareLogic = new CompareLogic();
        ComparisonResult result = compareLogic.Compare(this, other);

        return result.AreEqual;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Misc);
    }
}
