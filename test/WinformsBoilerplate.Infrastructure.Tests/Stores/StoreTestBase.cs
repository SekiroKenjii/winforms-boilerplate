namespace WinformsBoilerplate.Infrastructure.Tests.Stores;

/// <summary>
/// Base class for store tests that provides common testing utilities.
/// </summary>
public abstract class StoreTestBase : IDisposable
{
    protected readonly Fixture Fixture;
    protected readonly Mock<ILogger> MockLogger;

    protected StoreTestBase()
    {
        Fixture = new Fixture();
        MockLogger = new Mock<ILogger>();
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
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
        Assert.NotNull(testData);
    }
}
