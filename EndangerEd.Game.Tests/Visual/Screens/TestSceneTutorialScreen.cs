using EndangerEd.Game.Screens;
using osu.Framework.Allocation;

namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneTutorialScreen : EndangerEdTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AddStep("load screen", () => MainScreenStack.MainScreenStack.Push(new TutorialScreen()));
        AddStep("clear all screens", MainScreenStack.ClearMainScreenStack);
    }
}
