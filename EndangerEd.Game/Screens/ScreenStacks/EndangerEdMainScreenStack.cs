using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.Games;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens.ScreenStacks;

public partial class EndangerEdMainScreenStack : ScreenStack
{
    [Resolved]
    private SessionStore sessionStore { get; set; }

    public EndangerEdGameSessionScreenStack GameScreenStack;

    public ScreenStack MainScreenStack;

    private Container loadingContainer;
    private SpriteIcon loadingIcon;

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
            },
            loadingContainer = new Container()
            {
                Alpha = 0,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Size = new Vector2(80, 80),
                Margin = new MarginPadding()
                {
                    Bottom = 10,
                    Left = 10
                },
                Masking = true,
                CornerRadius = 10,
                Children = new Drawable[]
                {
                    loadingIcon = new SpriteIcon()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Solid.CircleNotch,
                        Size = new Vector2(50),
                        Colour = Colour4.White,
                        Alpha = 1f
                    },
                    new Box()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.DarkGray,
                        Alpha = 0.5f
                    }
                },
            },
            new EndangerEdSpriteText()
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Text = "Debug build".ToUpper(),
                Colour = Colour4.White,
                Font = EndangerEdFont.GetFont(typeface: EndangerEdFont.Typeface.JosefinSans, size: 20, weight: EndangerEdFont.FontWeight.Bold),
                Margin = new MarginPadding { Bottom = 10, Left = 10 },
                Alpha = DebugUtils.IsDebugBuild ? 0.5f : 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadingIcon.RotateTo(0).Then()
                   .RotateTo(360, 1000, Easing.InOutSine)
                   .Loop();
        sessionStore.IsLoading.BindValueChanged(isLoading =>
        {
            Scheduler.Add(() => loadingContainer.FadeTo(isLoading.NewValue ? 1 : 0, 500, Easing.OutQuint));
        }, true);
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

            case QuestionMode.Cannon:
                GameScreenStack.MainScreenStack.Push(new CannonGameScreen(question));
                break;

            case QuestionMode.Bucket:
                GameScreenStack.MainScreenStack.Push(new BucketGameScreen(question));
                break;

            case QuestionMode.TakePicture:
                GameScreenStack.MainScreenStack.Push(new TakePictureGameScreen(question));
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
