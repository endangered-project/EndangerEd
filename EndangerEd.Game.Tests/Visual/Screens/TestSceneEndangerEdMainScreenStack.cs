namespace EndangerEd.Game.Tests.Visual.Screens;

public partial class TestSceneEndangerEdMainScreenStack : EndangerEdTestScene
{
    protected override void LoadComplete()
    {
        base.LoadComplete();
        AddStep("swap mode", MainScreenStack.SwapScreenStack);
    }
}
