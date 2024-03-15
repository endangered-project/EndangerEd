using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.Games;
using osu.Framework.Screens;

namespace EndangerEd.Game.Tests.Visual.GameScreens;

public partial class TestSceneFourChoiceGameScreen : EndangerEdTestScene
{
    protected override void LoadComplete()
    {
        var gameScreen = new FourChoiceGameScreen(new Question()
        {
            QuestionText = "What is the question?",
            Choices = ["Choice 1", "Choice 2", "Choice 3", "Choice 4"],
            Answer = "Choice 2",
            ContentType = ContentType.Text,
            QuestionMode = QuestionMode.FourChoice
        });
        base.LoadComplete();
        AddStep("add game screen", () =>
        {
            MainScreenStack.SwapScreenStack();
            MainScreenStack.GameScreenStack.MainScreenStack.Push(gameScreen);
        });
        AddStep("start countdown", () => GameSessionStore.StopwatchClock.Start());
        AddStep("set life to 1", () => GameSessionStore.Life.Value = 1);
        AddStep("clear game screen", gameScreen.Exit);
    }
}
