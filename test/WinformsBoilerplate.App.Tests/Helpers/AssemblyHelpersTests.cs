namespace WinformsBoilerplate.App.Tests.Helpers;

public class AssemblyHelpersTests
{
    [Fact]
    public void ValidateLibVersions_ShouldReturnVersionInfo()
    {
        // Arrange & Act
        AssemblyHelpers.ValidateLibVersions(out Version requiredVersion);

        // Assert
        requiredVersion.Should().NotBeNull();
        requiredVersion.Should().Be(new Version(Application.ProductVersion));
    }

    [Fact]
    public void ValidateLibVersions_ShouldExecuteWithoutThrowing()
    {
        // Arrange & Act & Assert
        var act = () => AssemblyHelpers.ValidateLibVersions(out Version _);
        act.Should().NotThrow();
    }
}
