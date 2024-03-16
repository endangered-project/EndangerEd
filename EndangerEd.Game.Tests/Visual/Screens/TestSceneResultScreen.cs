using EndangerEd.Game.Screens.Games;
using osu.Framework.Allocation;

namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneResultScreen : EndangerEdTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        MainScreenStack.MainScreenStack.Push(new ResultScreen());
    }
}
