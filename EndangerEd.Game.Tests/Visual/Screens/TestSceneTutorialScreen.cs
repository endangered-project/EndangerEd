using EndangerEd.Game.Screens;
using NUnit.Framework;

namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneTutorialScreen : EndangerEdTestScene
{
    [Test]
    public void TestTutorialScreen()
    {
        AddStep("load screen", () => MainScreenStack.MainScreenStack.Push(new TutorialScreen()));
    }
}
