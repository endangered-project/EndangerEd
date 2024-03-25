using EndangerEd.Game.Screens;
using osu.Framework.Allocation;

namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneMainMenuScreen : EndangerEdTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        MainScreenStack.MainScreenStack.Push(new MainMenuScreen());
        AddStep("log in", () => SessionStore.IsLoggedIn.Value = true);
    }
}
