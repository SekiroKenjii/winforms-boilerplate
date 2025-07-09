namespace WinformsBoilerplate.Infrastructure.Tests.Stores;

/// <summary>
/// Base class for store tests that provides common testing utilities.
/// </summary>
public abstract class StoreTestBase
{
    protected readonly Fixture Fixture;
    protected readonly Mock<ILogger> MockLogger;

    protected StoreTestBase()
    {
        Fixture = new Fixture();
        MockLogger = new Mock<ILogger>();
    }
}

/// <summary>
/// Sample test class for infrastructure stores.
/// Add specific store tests here based on your actual stores.
/// </summary>
public class SampleStoreTests : StoreTestBase
{
    [Fact]
    public void SampleStoreTest_ShouldPass()
    {
        // Arrange
        var testData = Fixture.Create<string>();

        // Act & Assert
        testData.Should().NotBeNull();
    }
}
