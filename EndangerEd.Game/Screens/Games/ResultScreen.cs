using EndangerEd.Game.API;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Screens.ScreenStacks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osuTK;

namespace EndangerEd.Game.Screens.Games;

public partial class ResultScreen : EndangerEdScreen
{
    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private APIEndpointConfig endpointConfig { get; set; }

    private SpriteIcon loadingIcon;
    private Container loadingContainer;
    private FillFlowContainer leaderboardNameContainer;
    private FillFlowContainer leaderboardScoreContainer;
    private FillFlowContainer resultContainer;
    private SpriteText sessionIdText;
    private EndangerEdButton backToMenuButton;
    private EndangerEdButton playAgainButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            // leaderboard
            leaderboardNameContainer = new FillFlowContainer()
            {
                Direction = FillDirection.Vertical,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding()
                {
                    Top = 80,
                    Right = 500,
                },
                Spacing = new Vector2(10),
                Alpha = 0,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = "LEADERBOARD",
                        Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, size: 50, weight: EndangerEdFont.FontWeight.Bold)
                    },
                    new SpriteText
                    {
                        Text = "1. Player 1",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "2. Player 2",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "3. Player 3",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "4. Player 4",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "5. Player 5",
                        Font = new FontUsage(size: 30),
                    },
                    new EndangerEdButton("Full leaderboard")
                    {
                        Size = new Vector2(150, 50),
                        Action = () => host.OpenUrlExternally(endpointConfig.GameUrl + "leaderboard")
                    }
                }
            },
            // leaderboard score
            leaderboardScoreContainer = new FillFlowContainer()
            {
                Direction = FillDirection.Vertical,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding()
                {
                    Top = 140,
                    Right = 100,
                },
                Spacing = new Vector2(10),
                Alpha = 0,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = "100",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "90",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "80",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "70",
                        Font = new FontUsage(size: 30),
                    },
                    new SpriteText
                    {
                        Text = "60",
                        Font = new FontUsage(size: 30),
                    },
                }
            },
            // score
            resultContainer = new FillFlowContainer()
            {
                Direction = FillDirection.Vertical,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Margin = new MarginPadding()
                {
                    Top = 80,
                    Left = 100,
                },
                Spacing = new Vector2(10),
                Alpha = 0,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = "RESULT",
                        Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, size: 50, weight: EndangerEdFont.FontWeight.Bold)
                    },
                    new SpriteText
                    {
                        Text = "Score : 100",
                        Font = new FontUsage(size: 30)
                    },
                    new SpriteText
                    {
                        Text = "Rank : 1",
                        Font = new FontUsage(size: 30)
                    },
                    new TextFlowContainer()
                    {
                        Text = "See the full result with answer and your play history in EndangerEd website",
                        Width = 360,
                        Height = 60,
                        Scale = new Vector2(1.2f),
                        Padding = new MarginPadding()
                        {
                            Top = 20
                        }
                    },
                    new EndangerEdButton("Full Result")
                    {
                        Size = new Vector2(110, 50),
                        // TODO: Change URL
                        Action = () => host.OpenUrlExternally(endpointConfig.GameUrl + "leaderboard")
                    }
                }
            },
            // Session ID
            sessionIdText = new SpriteText
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding()
                {
                    Bottom = 10,
                    Left = 10
                },
                Text = "Session ID : 0",
                Colour = Colour4.Gray,
                Font = EndangerEdFont.GetFont(size: 20),
                Alpha = 0
            },
            // Back to menu
            backToMenuButton = new EndangerEdButton("Back to menu")
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Margin = new MarginPadding()
                {
                    Bottom = 10,
                    Right = 10
                },
                Size = new Vector2(200, 50),
                Action = () =>
                {
                    mainScreenStack.MainScreenStack.Push(new MainMenuScreen());
                },
                Alpha = 0
            },
            // Play again
            playAgainButton = new EndangerEdButton("Play again")
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Margin = new MarginPadding()
                {
                    Bottom = 10,
                    Right = 220
                },
                Size = new Vector2(200, 50),
                Action = () =>
                {
                    mainScreenStack.MainScreenStack.Push(new LoadingScreen());
                },
                Alpha = 0
            },
            // Loading result badge
            loadingContainer = new Container()
            {
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
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        loadingIcon.RotateTo(0).Then()
                   .RotateTo(360, 1000, Easing.InOutSine)
                   .Loop();
        // TODO: Fetch result from API before showing the result
        resultContainer.FadeInFromZero(1000, Easing.OutQuint);
        leaderboardScoreContainer.FadeInFromZero(1500, Easing.OutQuint);
        leaderboardNameContainer.FadeInFromZero(1500, Easing.OutQuint);
        sessionIdText.FadeInFromZero(2000, Easing.OutQuint);
        playAgainButton.FadeInFromZero(2500, Easing.OutQuint);
        backToMenuButton.FadeInFromZero(3000, Easing.OutQuint);
    }
}
