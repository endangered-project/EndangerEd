using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.Games;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens.ScreenStacks;

public partial class EndangerEdMainScreenStack : ScreenStack
{
    [Resolved]
    private SessionStore sessionStore { get; set; }

    public EndangerEdGameSessionScreenStack GameScreenStack;

    public ScreenStack MainScreenStack;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new BackgroundScreen()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            },
            GameScreenStack = new EndangerEdGameSessionScreenStack
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0f
            },
            MainScreenStack = new ScreenStack
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            }
        };
    }

    public void SwapScreenStack(double delayBetweenSwap = 0)
    {
        if (GameScreenStack.Alpha != 0f)
        {
            GameScreenStack.Hide();
            Scheduler.AddDelayed(() => MainScreenStack.Show(), delayBetweenSwap);
        }
        else
        {
            GameScreenStack.Show();
            Scheduler.AddDelayed(() => MainScreenStack.Hide(), delayBetweenSwap);
        }
    }

    public void PushQuestionScreen(Question question)
    {
        switch (question.QuestionMode)
        {
            case QuestionMode.FourChoice:
                GameScreenStack.MainScreenStack.Push(new FourChoiceGameScreen(question));
                break;

            default:
                GameScreenStack.MainScreenStack.Push(new FourChoiceGameScreen(question));
                break;
        }
    }

    public void ResetGameScreenStack()
    {
        GameScreenStack.MainScreenStack = new ScreenStack
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.875f, 0.875f),
            Margin = new MarginPadding(10),
            Name = "Main screen"
        };
    }
}
