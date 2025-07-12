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
        Assert.Equal(5, results.Count);
        Assert.Equal((1, 0), results[0]);
        Assert.Equal((2, 1), results[1]);
        Assert.Equal((3, 2), results[2]);
        Assert.Equal((4, 3), results[3]);
        Assert.Equal((5, 4), results[4]);
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
        Assert.False(actionExecuted);
    }

    [Fact]
    public void ForEach_WithNullEnumerable_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<int>? source = null;

        // Act & Assert
        var act = () => source!.ForEach((_, _) => { });
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void ForEach_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Action<int, int>? action = null;

        // Act & Assert
        var act = () => source.ForEach(action!);
        Assert.Throws<ArgumentNullException>(act);
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
        Assert.Equal(source.Length, count);
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] { new[] { "a", "b", "c" } };
        yield return new object[] { new[] { 1.0, 2.0, 3.0 } };
    }
}
