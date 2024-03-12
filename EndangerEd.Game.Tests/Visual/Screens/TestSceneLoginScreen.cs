using EndangerEd.Game.Screens;
using osu.Framework.Allocation;

namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneLoginScreen : EndangerEdTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        MainScreenStack.Push(new LoginScreen());
    }
}
