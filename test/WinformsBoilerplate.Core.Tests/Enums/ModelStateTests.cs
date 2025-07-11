namespace WinformsBoilerplate.Core.Tests.Enums;

public class ModelStateTests
{
    [Fact]
    public void ModelState_ShouldHaveExpectedValues()
    {
        // Verify that the ModelState enum has the expected values
        var enumValues = Enum.GetValues<ModelState>();
        Assert.NotEmpty(enumValues);
        Assert.Contains(ModelState.Init, enumValues);
        Assert.Contains(ModelState.None, enumValues);
        Assert.Contains(ModelState.Modified, enumValues);
    }

    [Theory]
    [InlineData(ModelState.Init)]
    [InlineData(ModelState.None)]
    [InlineData(ModelState.Modified)]
    public void ModelState_ShouldBeValidEnumValue(ModelState state)
    {
        // Verify that the enum values are defined
        Assert.True(Enum.IsDefined(typeof(ModelState), state));
    }
}
