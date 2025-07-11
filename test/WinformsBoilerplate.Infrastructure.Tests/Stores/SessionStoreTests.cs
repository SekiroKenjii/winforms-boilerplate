using System.Collections.Concurrent;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Infrastructure.Stores;

namespace WinformsBoilerplate.Infrastructure.Tests.Stores;

public class SessionStoreTests : IDisposable
{
    private readonly SessionStore _sessionStore;

    public SessionStoreTests()
    {
        _sessionStore = new SessionStore();
    }

    public void Dispose()
    {
        _sessionStore?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var store = new SessionStore();

        // Assert
        Assert.NotNull(store);
        Assert.IsAssignableFrom<ISessionStore>(store);
        Assert.IsAssignableFrom<IKeyValueStore>(store);
    }

    [Fact]
    public void Set_WithValidKeyAndValue_ShouldStoreValue()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";

        // Act
        _sessionStore.Set(key, value);

        // Assert
        var result = _sessionStore.Get<string>(key);
        Assert.Equal(value, result);
    }

    [Fact]
    public void Set_WithExistingKey_ShouldUpdateValue()
    {
        // Arrange
        const string key = "testKey";
        const string originalValue = "originalValue";
        const string newValue = "newValue";

        _sessionStore.Set(key, originalValue);

        // Act
        _sessionStore.Set(key, newValue);

        // Assert
        var result = _sessionStore.Get<string>(key);
        Assert.Equal(newValue, result);
    }

    [Theory]
    [InlineData("stringValue")]
    [InlineData(42)]
    [InlineData(true)]
    [InlineData(3.14)]
    public void Set_WithDifferentTypes_ShouldStoreCorrectly<T>(T value)
    {
        // Arrange
        const string key = "testKey";

        // Act
        _sessionStore.Set(key, value);

        // Assert
        var result = _sessionStore.Get<T>(key);
        Assert.Equal(value, result);
    }

    [Fact]
    public void Set_WithNullValue_ShouldStoreNull()
    {
        // Arrange
        const string key = "testKey";
        string? value = null;

        // Act
        _sessionStore.Set(key, value);

        // Assert
        var result = _sessionStore.Get<string>(key);
        Assert.Null(result);
    }

    [Fact]
    public void Get_WithExistingKey_ShouldReturnValue()
    {
        // Arrange
        const string key = "existingKey";
        const string value = "existingValue";
        _sessionStore.Set(key, value);

        // Act
        var result = _sessionStore.Get<string>(key);

        // Assert
        Assert.Equal(value, result);
        Assert.NotNull(result);
    }

    [Fact]
    public void Get_WithNonExistentKey_ShouldReturnDefault()
    {
        // Arrange
        const string key = "nonExistentKey";

        // Act
        var result = _sessionStore.Get<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Get_WithIncompatibleType_ShouldReturnDefault()
    {
        // Arrange
        const string key = "testKey";
        const string value = "stringValue";
        _sessionStore.Set(key, value);

        // Act
        var result = _sessionStore.Get<int>(key);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Get_WithComplexObject_ShouldReturnCorrectValue()
    {
        // Arrange
        const string key = "testKey";
        var value = new { Name = "Test", Age = 25 };
        _sessionStore.Set(key, value);

        // Act
        var result = _sessionStore.Get<object>(key);

        // Assert
        Assert.Equal(value, result);
    }

    [Fact]
    public void Remove_WithExistingKey_ShouldRemoveItem()
    {
        // Arrange
        const string key = "testKey";
        const string value = "testValue";
        _sessionStore.Set(key, value);

        // Act
        _sessionStore.Remove(key);

        // Assert
        var result = _sessionStore.Get<string>(key);
        Assert.Null(result);
    }

    [Fact]
    public void Remove_WithNonExistentKey_ShouldNotThrow()
    {
        // Arrange
        const string key = "nonExistentKey";

        // Act & Assert
        var act = () => _sessionStore.Remove(key);
        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        // Arrange
        _sessionStore.Set("key1", "value1");
        _sessionStore.Set("key2", "value2");
        _sessionStore.Set("key3", "value3");

        // Act
        _sessionStore.Clear();

        // Assert
        Assert.Null(_sessionStore.Get<string>("key1"));
        Assert.Null(_sessionStore.Get<string>("key2"));
        Assert.Null(_sessionStore.Get<string>("key3"));
    }

    [Fact]
    public void Inc_WithNewKey_ShouldSetToSpecifiedValue()
    {
        // Arrange
        const string key = "counterKey";
        const int incrementValue = 5;

        // Act
        _sessionStore.Inc(key, incrementValue);

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(incrementValue, result);
    }

    [Fact]
    public void Inc_WithExistingKey_ShouldIncrementValue()
    {
        // Arrange
        const string key = "counterKey";
        const int initialValue = 10;
        const int incrementValue = 3;
        _sessionStore.Set(key, initialValue);

        // Act
        _sessionStore.Inc(key, incrementValue);

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(initialValue + incrementValue, result);
    }

    [Fact]
    public void Inc_WithDefaultValue_ShouldIncrementByOne()
    {
        // Arrange
        const string key = "counterKey";
        const int initialValue = 5;
        _sessionStore.Set(key, initialValue);

        // Act
        _sessionStore.Inc(key);

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(initialValue + 1, result);
    }

    [Fact]
    public void Dec_WithNewKey_ShouldSetToNegativeSpecifiedValue()
    {
        // Arrange
        const string key = "counterKey";
        const int decrementValue = 5;

        // Act
        _sessionStore.Dec(key, decrementValue);

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(-decrementValue, result);
    }

    [Fact]
    public void Dec_WithExistingKey_ShouldDecrementValue()
    {
        // Arrange
        const string key = "counterKey";
        const int initialValue = 10;
        const int decrementValue = 3;
        _sessionStore.Set(key, initialValue);

        // Act
        _sessionStore.Dec(key, decrementValue);

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(initialValue - decrementValue, result);
    }

    [Fact]
    public void Dec_WithDefaultValue_ShouldDecrementByOne()
    {
        // Arrange
        const string key = "counterKey";
        const int initialValue = 5;
        _sessionStore.Set(key, initialValue);

        // Act
        _sessionStore.Dec(key);

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(initialValue - 1, result);
    }

    [Fact]
    public void Inc_Dec_ChainedOperations_ShouldWorkCorrectly()
    {
        // Arrange
        const string key = "counterKey";
        const int initialValue = 10;
        _sessionStore.Set(key, initialValue);

        // Act
        _sessionStore.Inc(key, 5);  // 10 + 5 = 15
        _sessionStore.Dec(key, 3);  // 15 - 3 = 12
        _sessionStore.Inc(key);     // 12 + 1 = 13
        _sessionStore.Dec(key);     // 13 - 1 = 12

        // Assert
        var result = _sessionStore.Get<int>(key);
        Assert.Equal(12, result);
    }

    [Fact]
    public void Dispose_ShouldClearStore()
    {
        // Arrange
        _sessionStore.Set("key1", "value1");
        _sessionStore.Set("key2", "value2");

        // Act
        _sessionStore.Dispose();

        // Assert
        Assert.Null(_sessionStore.Get<string>("key1"));
        Assert.Null(_sessionStore.Get<string>("key2"));
    }

    [Fact]
    public void Dispose_ShouldSetValuesToNull()
    {
        // Arrange
        const string key = "testKey";
        _sessionStore.Set(key, "testValue");

        // Act
        _sessionStore.Dispose();

        // Assert
        var result = _sessionStore.Get<string>(key);
        Assert.Null(result);
    }

    [Fact]
    public void MultipleOperations_ShouldMaintainDataIntegrity()
    {
        // Arrange
        const string stringKey = "stringKey";
        const string intKey = "intKey";
        const string boolKey = "boolKey";

        // Act
        _sessionStore.Set(stringKey, "test string");
        _sessionStore.Set(intKey, 42);
        _sessionStore.Set(boolKey, true);

        _sessionStore.Inc(intKey, 8);  // 42 + 8 = 50

        // Assert
        Assert.Equal("test string", _sessionStore.Get<string>(stringKey));
        Assert.Equal(50, _sessionStore.Get<int>(intKey));
        Assert.True(_sessionStore.Get<bool>(boolKey));
    }
}
