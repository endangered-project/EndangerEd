using NUnit.Framework;

namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneSettingsScreenStack : EndangerEdTestScene
{
    [Test]
    public void TestSettingsScreenStack()
    {
        AddStep("toggle visibility", () => SettingsScreenStack.ToggleVisibility());
    }
}
