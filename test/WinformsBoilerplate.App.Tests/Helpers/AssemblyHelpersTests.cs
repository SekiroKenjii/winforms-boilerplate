namespace WinformsBoilerplate.App.Tests.Helpers;

public class AssemblyHelpersTests
{
    [Fact]
    public void ValidateLibVersions_ShouldReturnVersionInfo()
    {
        // Arrange & Act
        AssemblyHelpers.ValidateLibVersions(out Version requiredVersion);

        // Assert
        Assert.NotNull(requiredVersion);
        Assert.Equal(new Version(Application.ProductVersion), requiredVersion);
    }

    [Fact]
    public void ValidateLibVersions_ShouldExecuteWithoutThrowing()
    {
        // Arrange & Act & Assert
        var exception = Record.Exception(() => AssemblyHelpers.ValidateLibVersions(out Version _));
        Assert.Null(exception);
    }
}
