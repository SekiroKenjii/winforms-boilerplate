namespace WinformsBoilerplate.Core.Tests.Enums;

public class ModelStateTests
{
    [Fact]
    public void ModelState_ShouldHaveExpectedValues()
    {
        // Verify that the ModelState enum has the expected values
        var enumValues = Enum.GetValues<ModelState>();
        enumValues.Should().NotBeEmpty();
        enumValues.Should().Contain(ModelState.Init);
        enumValues.Should().Contain(ModelState.None);
        enumValues.Should().Contain(ModelState.Modified);
    }

    [Theory]
    [InlineData(ModelState.Init)]
    [InlineData(ModelState.None)]
    [InlineData(ModelState.Modified)]
    public void ModelState_ShouldBeValidEnumValue(ModelState state)
    {
        // Verify that the enum values are defined
        Enum.IsDefined(typeof(ModelState), state).Should().BeTrue();
    }
}
