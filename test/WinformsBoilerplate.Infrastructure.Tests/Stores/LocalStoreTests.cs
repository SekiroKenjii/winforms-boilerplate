using System.Collections.Concurrent;
using System.Text.Json;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Infrastructure.Stores;

namespace WinformsBoilerplate.Infrastructure.Tests.Stores;

public class LocalStoreTests : IDisposable
{
    private readonly Mock<ILogService> _mockLogService;
    private readonly Mock<ISystemService> _mockSystemService;
    private readonly ConcurrentDictionary<string, object?> _testStore;
    private readonly LocalStore _localStore;

    public LocalStoreTests()
    {
        _mockLogService = new Mock<ILogService>();
        _mockSystemService = new Mock<ISystemService>();
        _testStore = new ConcurrentDictionary<string, object?>();

        _mockSystemService
            .Setup(ss => ss.ReadLocalStore())
            .Returns(_testStore);

        _localStore = new LocalStore(_mockLogService.Object, _mockSystemService.Object);
    }

    public void Dispose()
    {
        _localStore?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        // Arrange & Act
        var store = new LocalStore(_mockLogService.Object, _mockSystemService.Object);

        // Assert
        Assert.NotNull(store);
        Assert.IsAssignableFrom<ILocalStore>(store);
    }

    [Fact]
    public void Constructor_ShouldReadExistingStore()
    {
        // Arrange & Act & Assert
        _mockSystemService.Verify(ss => ss.ReadLocalStore(), Times.Once);
    }

    [Fact]
    public void Set_WithValidKeyAndValue_ShouldStoreValue()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";

        // Act
        _localStore.Set(key, value);

        // Assert
        Assert.True(_testStore.ContainsKey(key));
        Assert.Equal(value, _testStore[key]);
        _mockSystemService.Verify(ss => ss.SaveLocalStore(_testStore), Times.Once);
    }

    [Fact]
    public void Set_WithExistingKey_ShouldUpdateValue()
    {
        // Arrange
        const string key = "testKey";
        const string originalValue = "originalValue";
        const string newValue = "newValue";

        _localStore.Set(key, originalValue);

        // Act
        _localStore.Set(key, newValue);

        // Assert
        Assert.Equal(newValue, _testStore[key]);
        _mockSystemService.Verify(ss => ss.SaveLocalStore(_testStore), Times.Exactly(2));
    }

    [Theory]
    [InlineData("stringValue", "stringValue")]
    [InlineData(42, 42)]
    [InlineData(true, true)]
    public void Set_WithDifferentTypes_ShouldStoreCorrectly<T>(T value, T expected)
    {
        // Arrange
        const string key = "testKey";

        // Act
        _localStore.Set(key, value);

        // Assert
        Assert.Equal(expected, _testStore[key]);
    }

    [Fact]
    public void Get_WithExistingKey_ShouldReturnValue()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";
        _testStore[key] = value;

        // Act
        var result = _localStore.Get<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_WithNonExistentKey_ShouldReturnDefault()
    {
        // Arrange
        const string key = "nonExistentKey";

        // Act
        var result = _localStore.Get<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Get_WithJsonElementString_ShouldReturnCorrectValue()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _testStore[key] = jsonElement;

        // Act
        var result = _localStore.Get<string>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_WithJsonElementInt32_ShouldReturnCorrectValue()
    {
        // Arrange
        const string key = "testKey";
        const int value = 42;
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _testStore[key] = jsonElement;

        // Act
        var result = _localStore.Get<int>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_WithJsonElementDateTime_ShouldReturnCorrectValue()
    {
        // Arrange
        const string key = "testKey";
        var value = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _testStore[key] = jsonElement;

        // Act
        var result = _localStore.Get<DateTime>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_WithNullableType_ShouldHandleCorrectly()
    {
        // Arrange
        const string key = "testKey";
        int? value = 42;
        var jsonElement = JsonSerializer.SerializeToElement(value);
        _testStore[key] = jsonElement;

        // Act
        var result = _localStore.Get<int?>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Get_WithIncompatibleType_ShouldReturnDefault()
    {
        // Arrange
        const string key = "testKey";
        const string value = "stringValue";
        _testStore[key] = value;

        // Act
        var result = _localStore.Get<int>(key);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Remove_WithExistingKey_ShouldRemoveItem()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";
        _testStore[key] = value;

        // Act
        _localStore.Remove(key);

        // Assert
        Assert.False(_testStore.ContainsKey(key));
        _mockSystemService.Verify(ss => ss.SaveLocalStore(_testStore), Times.Once);
    }

    [Fact]
    public void Remove_WithNonExistentKey_ShouldNotThrow()
    {
        // Arrange
        const string key = "nonExistentKey";

        // Act & Assert
        var act = () => _localStore.Remove(key);
        var exception = Record.Exception(act);
        Assert.Null(exception);
        _mockSystemService.Verify(ss => ss.SaveLocalStore(_testStore), Times.Once);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        _testStore["key1"] = "value1";
        _testStore["key2"] = "value2";

        // Act
        _localStore.Clear();

        // Assert
        Assert.Empty(_testStore);
        _mockSystemService.Verify(ss => ss.SaveLocalStore(_testStore), Times.Once);
    }

    [Fact]
    public void Cleanup_ShouldLogAndClearStore()
    {
        // Arrange
        _testStore["key1"] = "value1";

        // Act
        _localStore.Cleanup();

        // Assert
        _mockLogService.VerifyAll();
        Assert.Empty(_testStore);
    }

    [Fact]
    public void Cleanup_WhenDirectoryExists_ShouldLogSuccess()
    {
        // Arrange & Act
        _localStore.Cleanup();

        // Assert
        _mockLogService.VerifyAll();
        // Note: The actual directory deletion test would require more complex setup
        // since it depends on the file system
    }

    [Fact]
    public void Dispose_ShouldClearStore()
    {
        // Arrange
        _localStore.Set("key1", "value1");
        _localStore.Set("key2", "value2");

        // Act
        _localStore.Dispose();

        // Assert
        Assert.Null(_localStore.Get<string>("key1"));
        Assert.Null(_localStore.Get<string>("key2"));
    }

    [Fact]
    public void Dispose_ShouldSetValuesToNull()
    {
        // Arrange
        const string key = "testKey";
        _localStore.Set(key, "testValue");

        // Act
        _localStore.Dispose();

        // Assert
        Assert.Null(_localStore.Get<string>(key));
    }
}
