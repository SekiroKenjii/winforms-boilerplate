namespace WinformsBoilerplate.Core.Tests.Extensions;

public class EnumerableExtensionsTests
{
    [Fact]
    public void ForEach_WithValidEnumerableAndAction_ShouldExecuteActionForEachElement()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var results = new List<(int value, int index)>();

        // Act
        source.ForEach((value, index) => results.Add((value, index)));

        // Assert
        results.Should().HaveCount(5);
        results[0].Should().Be((1, 0));
        results[1].Should().Be((2, 1));
        results[2].Should().Be((3, 2));
        results[3].Should().Be((4, 3));
        results[4].Should().Be((5, 4));
    }

    [Fact]
    public void ForEach_WithEmptyEnumerable_ShouldNotExecuteAction()
    {
        // Arrange
        var source = Array.Empty<int>();
        var actionExecuted = false;

        // Act
        source.ForEach((_, _) => actionExecuted = true);

        // Assert
        actionExecuted.Should().BeFalse();
    }

    [Fact]
    public void ForEach_WithNullEnumerable_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var act = () => source!.ForEach((_, _) => { });
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ForEach_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Action<int, int>? action = null;

        // Act & Assert
        var act = () => source.ForEach(action!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(GetTestData))]
    public void ForEach_WithDifferentTypes_ShouldWorkCorrectly<T>(T[] source)
    {
        // Arrange
        var count = 0;

        // Act
        source.ForEach((_, _) => count++);

        // Assert
        count.Should().Be(source.Length);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new[] { "a", "b", "c" } };
        yield return new object[] { new[] { 1.0, 2.0, 3.0 } };
    }
}
