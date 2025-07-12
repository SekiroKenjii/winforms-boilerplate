namespace WinformsBoilerplate.Core.Tests.Constants;

public class CommonTests
{
    [Fact]
    public void Constants_ShouldBeAccessible()
    {
        // This test ensures the Common constants class is accessible
        // Add specific tests based on the actual constants in the Common class

        // For now, just verify the namespace is accessible
        var type = typeof(Common);
        Assert.NotNull(type);
        Assert.Equal("WinformsBoilerplate.Core.Constants", type.Namespace);
    }
}
