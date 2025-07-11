namespace WinformsBoilerplate.App.Tests.Extensions;

/// <summary>
/// Base class for extension tests that provides common testing utilities.
/// </summary>
public abstract class ExtensionTestBase
{
    protected readonly Fixture Fixture;
    protected readonly Mock<IServiceCollection> MockServiceCollection;
    protected readonly Mock<IConfiguration> MockConfiguration;

    protected ExtensionTestBase()
    {
        Fixture = new Fixture();
        MockServiceCollection = new Mock<IServiceCollection>();
        MockConfiguration = new Mock<IConfiguration>();
    }
}

/// <summary>
/// Sample test class for app extensions.
/// Add specific extension tests here based on your actual extensions.
/// </summary>
public class SampleExtensionTests : ExtensionTestBase
{
    [Fact]
    public void SampleExtensionTest_ShouldPass()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        Assert.NotNull(services);
    }
}
