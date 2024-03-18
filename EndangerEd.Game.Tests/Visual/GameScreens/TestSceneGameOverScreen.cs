using EndangerEd.Game.Screens.Games;

namespace EndangerEd.Game.Tests.Visual.GameScreens;

public partial class TestSceneGameOverScreen : EndangerEdTestScene
{
    protected override void LoadComplete()
    {
        base.LoadComplete();
        AddStep("add game screen", () =>
        {
            MainScreenStack.SwapScreenStack();
            MainScreenStack.GameScreenStack.MainScreenStack.Push(new GameOverScreen());
        });
    }
}
