using KellermanSoftware.CompareNetObjects;

namespace WinformsBoilerplate.Core.Entities.Settings;

/// <summary>
/// Represents the application settings.
/// </summary>
public class AppSetting
{
    /// <summary>
    /// Gets or sets the miscellaneous settings for the application.
    /// </summary>
    public MiscSetting Misc { get; set; } = new();

    public bool Equals(AppSetting other)
    {
        if (other == null)
        {
            return false;
        }

        var compareLogic = new CompareLogic();
        ComparisonResult result = compareLogic.Compare(this, other);

        return result.AreEqual;
    }
}
