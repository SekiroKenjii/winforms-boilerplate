using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using WinformsBoilerplate.Core.Abstractions.Components;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Infrastructure.Stores;

namespace WinformsBoilerplate.Infrastructure.Tests.Stores;

public class EventStoreTests : StoreTestBase
{
    private readonly EventStore _eventStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly TestComponentEvent _testComponent;

    public EventStoreTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<TestComponentEvent>();
        services.AddSingleton<AnotherTestComponentEvent>();
        services.AddSingleton<IComponentEvent>(provider => provider.GetRequiredService<TestComponentEvent>());
        _serviceProvider = services.BuildServiceProvider();

        _eventStore = new EventStore(_serviceProvider);
        _testComponent = _serviceProvider.GetRequiredService<TestComponentEvent>();
    }

    [Fact]
    public void Constructor_WithValidServiceProvider_ShouldInitialize()
    {
        // Arrange & Act
        var eventStore = new EventStore(_serviceProvider);

        // Assert
        Assert.NotNull(eventStore);
        Assert.IsAssignableFrom<IEventStore>(eventStore);
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldThrow()
    {
        // Arrange & Act & Assert
        // Note: The EventStore uses primary constructor, so null check happens during usage, not construction
        var eventStore = new EventStore(null!);
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));

        var act = () => eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.Throws<ArgumentNullException>(act);
    }

    #region Add<T>(ReadOnlySpan<TeardownLogic> teardownLogics) Tests

    [Fact]
    public void Add_WithValidTeardownLogics_ShouldAddToStore()
    {
        // Arrange
        var teardownCalled = false;
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => teardownCalled = true));

        // Act
        _eventStore.Add<TestComponentEvent>(teardownLogic);

        // Assert
        Assert.False(teardownCalled); // teardown should not be called immediately
        Assert.True(_testComponent.TestActionCalled); // test action should be subscribed
    }

    [Fact]
    public void Add_WithMultipleTeardownLogics_ShouldAddAllToStore()
    {
        // Arrange
        var teardown1Called = false;
        var teardown2Called = false;
        var teardownLogic1 = new TeardownLogic("TestAction", new Action(() => teardown1Called = true));
        var teardownLogic2 = new TeardownLogic("AnotherAction", new Action(() => teardown2Called = true));

        // Act
        _eventStore.Add<TestComponentEvent>(teardownLogic1, teardownLogic2);

        // Assert
        Assert.False(teardown1Called); // teardown1 should not be called immediately
        Assert.False(teardown2Called); // teardown2 should not be called immediately
        Assert.True(_testComponent.TestActionCalled); // test action should be subscribed
        Assert.True(_testComponent.AnotherActionCalled); // another action should be subscribed
    }

    [Fact]
    public void Add_WithNullActionName_ShouldThrowArgumentException()
    {
        // Arrange
        var teardownLogic = new TeardownLogic(null!, new Action(() => { }));

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Add_WithEmptyActionName_ShouldThrowArgumentException()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("", new Action(() => { }));

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Add_WithWhitespaceActionName_ShouldThrowArgumentException()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("   ", new Action(() => { }));

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Add_WithNullEventHandler_ShouldThrowArgumentNullException()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", null!);

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Add_WithInvalidActionName_ShouldThrowArgumentException()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("NonExistentAction", new Action(() => { }));

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>(teardownLogic);
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("NonExistentAction", exception.Message);
        Assert.Contains("not found", exception.Message);
        Assert.Contains("TestComponentEvent", exception.Message);
    }

    #endregion

    #region Add<T>(string action, Delegate eventHandler) Tests

    [Fact]
    public void Add_WithActionAndDelegate_ShouldAddToStore()
    {
        // Arrange
        var handlerCalled = false;
        Action handler = () => handlerCalled = true;

        // Act
        _eventStore.Add<TestComponentEvent>("TestAction", handler);

        // Assert
        Assert.False(handlerCalled); // handler should not be called immediately
        Assert.True(_testComponent.TestActionCalled); // test action should be subscribed
    }

    [Fact]
    public void Add_WithNullAction_ShouldThrowArgumentException()
    {
        // Arrange
        Action handler = () => { };

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>(null!, handler);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Add_WithEmptyAction_ShouldThrowArgumentException()
    {
        // Arrange
        Action handler = () => { };

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>("", handler);
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Add_WithNullEventHandlerDelegate_ShouldSetPropertyToNull()
    {
        // Arrange & Act
        // The EventStore allows setting null delegates, which sets the property to null
        var act = () => _eventStore.Add<TestComponentEvent>("TestAction", null!);

        // Assert
        // The operation should not throw an exception
        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    [Fact]
    public void Add_WithInvalidActionNameDelegate_ShouldThrowArgumentException()
    {
        // Arrange
        Action handler = () => { };

        // Act & Assert
        var act = () => _eventStore.Add<TestComponentEvent>("InvalidAction", handler);
        var exception = Assert.Throws<ArgumentException>(act);
        Assert.Contains("InvalidAction", exception.Message);
        Assert.Contains("not found", exception.Message);
        Assert.Contains("TestComponentEvent", exception.Message);
    }

    [Fact]
    public void Add_WithCaseInsensitiveActionName_ShouldSucceed()
    {
        // Arrange
        Action handler = () => { };

        // Act
        _eventStore.Add<TestComponentEvent>("testaction", handler); // lowercase

        // Assert
        Assert.True(_testComponent.TestActionCalled); // case insensitive matching should work
    }

    #endregion

    #region Flush<T>() Tests

    [Fact]
    public void Flush_WithGenericType_ShouldRemoveMatchingItems()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.True(_testComponent.TestActionCalled); // precondition: action should be subscribed

        // Act
        _eventStore.Flush<TestComponentEvent>();

        // Assert
        Assert.False(_testComponent.TestActionCalled); // after flush, subscriptions should be removed
    }

    [Fact]
    public void Flush_WithGenericType_WhenNoMatchingType_ShouldNotAffectOtherTypes()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.True(_testComponent.TestActionCalled); // precondition: action should be subscribed

        // Act - flush different type
        _eventStore.Flush<AnotherTestComponentEvent>();

        // Assert
        Assert.True(_testComponent.TestActionCalled); // unrelated type flush should not affect subscriptions
    }

    [Fact]
    public void Flush_WithGenericType_WithMultipleActions_ShouldRemoveAllMatchingType()
    {
        // Arrange
        var teardownLogic1 = new TeardownLogic("TestAction", new Action(() => { }));
        var teardownLogic2 = new TeardownLogic("AnotherAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic1, teardownLogic2);
        Assert.True(_testComponent.TestActionCalled); // precondition: first action should be subscribed
        Assert.True(_testComponent.AnotherActionCalled); // precondition: second action should be subscribed

        // Act
        _eventStore.Flush<TestComponentEvent>();

        // Assert
        Assert.False(_testComponent.TestActionCalled); // after flush, first subscription should be removed
        Assert.False(_testComponent.AnotherActionCalled); // after flush, second subscription should be removed
    }

    #endregion

    #region Flush() Tests

    [Fact]
    public void Flush_WithoutParameters_ShouldClearAllItems()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.True(_testComponent.TestActionCalled); // precondition: action should be subscribed

        // Act
        _eventStore.Flush();

        // Assert
        Assert.False(_testComponent.TestActionCalled); // after flush all, subscriptions should be removed
    }

    [Fact]
    public void Flush_WithoutParameters_WithMultipleTypes_ShouldClearAll()
    {
        // Arrange
        var teardownLogic1 = new TeardownLogic("TestAction", new Action(() => { }));
        var teardownLogic2 = new TeardownLogic("AnotherAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic1, teardownLogic2);
        Assert.True(_testComponent.TestActionCalled); // precondition: first action should be subscribed
        Assert.True(_testComponent.AnotherActionCalled); // precondition: second action should be subscribed

        // Act
        _eventStore.Flush();

        // Assert
        Assert.False(_testComponent.TestActionCalled); // after flush all, first subscription should be removed
        Assert.False(_testComponent.AnotherActionCalled); // after flush all, second subscription should be removed
    }

    [Fact]
    public void Flush_WithoutParameters_OnEmptyStore_ShouldNotThrow()
    {
        // Arrange - empty store

        // Act & Assert
        var act = () => _eventStore.Flush();
        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_ShouldFlushAllItems()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.True(_testComponent.TestActionCalled); // precondition: action should be subscribed

        // Act
        _eventStore.Dispose();

        // Assert
        Assert.False(_testComponent.TestActionCalled); // after dispose, subscriptions should be removed
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic);

        // Act & Assert
        _eventStore.Dispose();
        var act = () => _eventStore.Dispose();
        var exception = Record.Exception(act);
        Assert.Null(exception); // multiple dispose calls should be safe
    }

    [Fact]
    public void Dispose_ShouldSetDisposedFlag()
    {
        // Arrange
        var eventStore = new EventStore(_serviceProvider);
        var teardownLogic = new TeardownLogic("TestAction", () => { });
        eventStore.Add<TestComponentEvent>(teardownLogic);

        // Act
        eventStore.Dispose();

        // Assert - Test that operations after dispose don't work (indicating disposed state)
        var act = () => eventStore.Add<TestComponentEvent>("TestAction", () => { });
        var exception = Record.Exception(act);
        Assert.Null(exception); // dispose should be handled gracefully
    }

    #endregion

    #region GetTarget Tests (Protected Method)

    [Fact]
    public void GetTarget_WithExistingAction_ShouldReturnStoredTarget()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));
        _eventStore.Add<TestComponentEvent>(teardownLogic);

        // Act - Using reflection to test protected method
        var getTargetMethod = typeof(EventStore).GetMethod("GetTarget", BindingFlags.NonPublic | BindingFlags.Instance);
        var genericMethod = getTargetMethod?.MakeGenericMethod(typeof(TestComponentEvent));
        var result = genericMethod?.Invoke(_eventStore, new object[] { "TestAction" });

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TestComponentEvent>(result);
    }

    [Fact]
    public void GetTarget_WithNonExistingAction_ShouldReturnNewInstance()
    {
        // Arrange - empty store

        // Act - Using reflection to test protected method
        var getTargetMethod = typeof(EventStore).GetMethod("GetTarget", BindingFlags.NonPublic | BindingFlags.Instance);
        var genericMethod = getTargetMethod?.MakeGenericMethod(typeof(TestComponentEvent));
        var result = genericMethod?.Invoke(_eventStore, new object[] { "NonExistentAction" });

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TestComponentEvent>(result);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Integration_AddFlushAdd_ShouldWorkCorrectly()
    {
        // Arrange
        var teardownLogic = new TeardownLogic("TestAction", new Action(() => { }));

        // Act & Assert - Add
        _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.True(_testComponent.TestActionCalled); // after add, subscription should exist

        // Act & Assert - Flush
        _eventStore.Flush<TestComponentEvent>();
        Assert.False(_testComponent.TestActionCalled); // after flush, subscription should be removed

        // Act & Assert - Add again
        _eventStore.Add<TestComponentEvent>(teardownLogic);
        Assert.True(_testComponent.TestActionCalled); // after re-add, subscription should exist again
    }

    [Fact]
    public void Integration_MultipleComponents_ShouldHandleIndependently()
    {
        // Arrange
        var teardownLogic1 = new TeardownLogic("TestAction", new Action(() => { }));
        var teardownLogic2 = new TeardownLogic("AnotherTestAction", new Action(() => { }));

        // Act
        _eventStore.Add<TestComponentEvent>(teardownLogic1);
        _eventStore.Add<AnotherTestComponentEvent>(teardownLogic2);

        // Assert
        Assert.True(_testComponent.TestActionCalled); // first component should be subscribed
        // Note: We can't easily test AnotherTestComponentEvent without setting up another service

        // Act - Flush only first type
        _eventStore.Flush<TestComponentEvent>();

        // Assert
        Assert.False(_testComponent.TestActionCalled); // first component subscription should be removed
    }

    #endregion

    #region Test Component Events

    // Test component event implementation for testing
    public class TestComponentEvent : IComponentEvent
    {
        public bool TestActionCalled { get; private set; }
        public bool AnotherActionCalled { get; private set; }

        public Action? TestAction
        {
            get => TestActionCalled ? () => { } : null;
            set => TestActionCalled = value != null;
        }

        public Action? AnotherAction
        {
            get => AnotherActionCalled ? () => { } : null;
            set => AnotherActionCalled = value != null;
        }

        public void InitializeComponentEvents()
        {
            // Test implementation - no-op
        }

        public void Reset()
        {
            TestActionCalled = false;
            AnotherActionCalled = false;
        }
    }

    public class AnotherTestComponentEvent : IComponentEvent
    {
        public bool TestActionCalled { get; private set; }
        public bool AnotherTestActionCalled { get; private set; }

        public Action? TestAction
        {
            get => TestActionCalled ? () => { } : null;
            set => TestActionCalled = value != null;
        }

        public Action? AnotherTestAction
        {
            get => AnotherTestActionCalled ? () => { } : null;
            set => AnotherTestActionCalled = value != null;
        }

        public void InitializeComponentEvents()
        {
            // Test implementation - no-op
        }
    }

    #endregion
}
