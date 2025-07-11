namespace WinformsBoilerplate.Infrastructure.Tests.Services;

/// <summary>
/// Base class for service tests that provides common testing utilities.
/// </summary>
public abstract class ServiceTestBase
{
    protected readonly Fixture Fixture;
    protected readonly Mock<ILogger> MockLogger;
    protected readonly Mock<IConfiguration> MockConfiguration;

    protected ServiceTestBase()
    {
        Fixture = new Fixture();
        MockLogger = new Mock<ILogger>();
        MockConfiguration = new Mock<IConfiguration>();
    }

    protected IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton(MockLogger.Object);
        services.AddSingleton(MockConfiguration.Object);
        return services;
    }
}

/// <summary>
/// Sample test class for infrastructure services.
/// Add specific service tests here based on your actual services.
/// </summary>
public class SampleServiceTests : ServiceTestBase
{
    [Fact]
    public void SampleTest_ShouldPass()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act & Assert
        Assert.NotNull(services);
        Assert.NotEmpty(services);
    }
}
