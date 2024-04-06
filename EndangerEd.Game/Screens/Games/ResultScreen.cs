using System;
using System.Threading;
using EndangerEd.Game.API;
using EndangerEd.Game.Audio;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Screens.ScreenStacks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

    [Resolved]
    private AudioPlayer audioPlayer { get; set; }

    private SpriteIcon loadingIcon;
    private Container loadingContainer;
    private FillFlowContainer leaderboardNameContainer;
    private FillFlowContainer leaderboardScoreContainer;
    private FillFlowContainer resultContainer;
    private SpriteText sessionIdText;
    private EndangerEdButton backToMenuButton;
    private EndangerEdButton playAgainButton;

    private SpriteText scoreText;
    private SpriteText rankAfter;
    private SpriteText rankChange;
    private SpriteText rightAnswer;
    private SpriteText wrongAnswer;

    private SpriteText playerName1;
    private SpriteText playerName2;
    private SpriteText playerName3;
    private SpriteText playerName4;
    private SpriteText playerName5;
    private SpriteText playerScore1;
    private SpriteText playerScore2;
    private SpriteText playerScore3;
    private SpriteText playerScore4;
    private SpriteText playerScore5;

    private SpriteText errorMessage;

    private BindableBool loadingComplete = new BindableBool();
    private int resultId;

    public ResultScreen(int resultId)
    {
        this.resultId = resultId;
    }

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
                    playerName1 = new SpriteText
                    {
                        Text = "1. Player 1",
                        Font = new FontUsage(size: 30),
                        Colour = Colour4.Gold
                    },
                    playerName2 = new SpriteText
                    {
                        Text = "2. Player 2",
                        Font = new FontUsage(size: 30),
                        Colour = Colour4.Silver
                    },
                    playerName3 = new SpriteText
                    {
                        Text = "3. Player 3",
                        Font = new FontUsage(size: 30),
                        Colour = Colour4.Brown
                    },
                    playerName4 = new SpriteText
                    {
                        Text = "4. Player 4",
                        Font = new FontUsage(size: 30),
                    },
                    playerName5 = new SpriteText
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
                    Right = 150,
                },
                Spacing = new Vector2(10),
                Alpha = 0,
                Children = new Drawable[]
                {
                    playerScore1 = new SpriteText
                    {
                        Text = "100",
                        Font = new FontUsage(size: 30),
                        Colour = Colour4.Gold
                    },
                    playerScore2 = new SpriteText
                    {
                        Text = "90",
                        Font = new FontUsage(size: 30),
                        Colour = Colour4.Silver
                    },
                    playerScore3 = new SpriteText
                    {
                        Text = "80",
                        Font = new FontUsage(size: 30),
                        Colour = Colour4.Brown
                    },
                    playerScore4 = new SpriteText
                    {
                        Text = "70",
                        Font = new FontUsage(size: 30),
                    },
                    playerScore5 = new SpriteText
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
                    scoreText = new SpriteText
                    {
                        Text = "Score : 100",
                        Font = new FontUsage(size: 30)
                    },
                    new FillFlowContainer()
                    {
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Height = 30,
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = "Rank :",
                                Font = new FontUsage(size: 30)
                            },
                            rankAfter = new SpriteText
                            {
                                Text = "1",
                                Font = new FontUsage(size: 30)
                            },
                            rankChange = new SpriteText
                            {
                                Text = "(0)",
                                Font = new FontUsage(size: 30),
                                Colour = Colour4.Gray
                            }
                        }
                    },
                    new FillFlowContainer()
                    {
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new SpriteIcon()
                            {
                                Icon = FontAwesome.Solid.CheckCircle,
                                Size = new Vector2(30),
                                Colour = Colour4.LightGreen
                            },
                            rightAnswer = new SpriteText
                            {
                                Text = "",
                                Font = new FontUsage(size: 30),
                                Colour = Colour4.LightGreen
                            },
                            new SpriteIcon()
                            {
                                Icon = FontAwesome.Solid.TimesCircle,
                                Size = new Vector2(30),
                                Colour = Colour4.Red
                            },
                            wrongAnswer = new SpriteText
                            {
                                Text = "",
                                Font = new FontUsage(size: 30),
                                Colour = Colour4.Red
                            }
                        }
                    },
                    new TextFlowContainer()
                    {
                        Text = "See the full result with answers and your play history on the EndangerEd website!",
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
                        Action = () => host.OpenUrlExternally(endpointConfig.GameUrl + "history/" + resultId)
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
                }
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
                },
            },
            // error message
            errorMessage = new SpriteText
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Text = "Failed to load result",
                Font = EndangerEdFont.GetFont(size: 20),
                Colour = Colour4.Red,
                Margin = new MarginPadding()
                {
                    Bottom = 10,
                    Left = 10
                },
                Alpha = 0
            }
        };

        loadingComplete.BindValueChanged(complete =>
        {
            if (complete.NewValue)
            {
                loadingContainer.FadeOut(500, Easing.OutQuint);
                resultContainer.FadeInFromZero(1500, Easing.OutQuint);
                leaderboardScoreContainer.FadeInFromZero(2000, Easing.OutQuint);
                leaderboardNameContainer.FadeInFromZero(2000, Easing.OutQuint);
                sessionIdText.FadeInFromZero(2500, Easing.OutQuint);
                playAgainButton.FadeInFromZero(3000, Easing.OutQuint);
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        loadingIcon.RotateTo(0).Then()
                   .RotateTo(360, 1000, Easing.InOutSine)
                   .Loop();
        sessionIdText.Text = "Session ID : " + resultId;
        audioPlayer.ChangeTrack("result.mp3");

        Thread thread = new Thread(() =>
        {
            try
            {
                var result = apiRequestManager.Get("history/" + resultId);
                var score = result["score"];
                var apiRankBefore = result["rank_before"];
                var apiRankAfter = result["rank_after"];
                var apiRightAnswer = result["right"];
                var apiWrongAnswer = result["wrong"];

                var leaderboard = apiRequestManager.Get("leaderboard");

                Scheduler.Add(() =>
                {
                    scoreText.Text = "Score : " + score;
                    rankAfter.Text = apiRankAfter.ToString();
                    rankChange.Text = "(" + (int.Parse(apiRankAfter.ToString()) - int.Parse(apiRankBefore.ToString())) + ")";
                    rightAnswer.Text = apiRightAnswer.ToString();
                    wrongAnswer.Text = apiWrongAnswer.ToString();

                    if (int.Parse(apiRankAfter.ToString()) - int.Parse(apiRankBefore.ToString()) > 0)
                    {
                        rankChange.Colour = Colour4.Red;
                    }
                    else if (int.Parse(apiRankAfter.ToString()) - int.Parse(apiRankBefore.ToString()) < 0)
                    {
                        rankChange.Colour = Colour4.LightGreen;
                    }

                    if (leaderboard.TryGetValue("username1", out var player1))
                    {
                        playerName1.Text = "1. " + player1;
                    }
                    else
                    {
                        playerName1.Text = "";
                        playerName1.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("username2", out var player2))
                    {
                        playerName2.Text = "2. " + player2;
                    }
                    else
                    {
                        playerName2.Text = "";
                        playerName2.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("username3", out var player3))
                    {
                        playerName3.Text = "3. " + player3;
                    }
                    else
                    {
                        playerName3.Text = "";
                        playerName3.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("username4", out var player4))
                    {
                        playerName4.Text = "4. " + player4;
                    }
                    else
                    {
                        playerName4.Text = "";
                        playerName4.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("username5", out var player5))
                    {
                        playerName5.Text = "5. " + player5;
                    }
                    else
                    {
                        playerName5.Text = "";
                        playerName5.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("score1", out var score1))
                    {
                        playerScore1.Text = score1.ToString();
                    }
                    else
                    {
                        playerScore1.Text = "";
                        playerScore1.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("score2", out var score2))
                    {
                        playerScore2.Text = score2.ToString();
                    }
                    else
                    {
                        playerScore2.Text = "";
                        playerScore2.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("score3", out var score3))
                    {
                        playerScore3.Text = score3.ToString();
                    }
                    else
                    {
                        playerScore3.Text = "";
                        playerScore3.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("score4", out var score4))
                    {
                        playerScore4.Text = score4.ToString();
                    }
                    else
                    {
                        playerScore4.Text = "";
                        playerScore4.Alpha = 0;
                    }

                    if (leaderboard.TryGetValue("score5", out var score5))
                    {
                        playerScore5.Text = score5.ToString();
                    }
                    else
                    {
                        playerScore5.Text = "";
                        playerScore5.Alpha = 0;
                    }

                    loadingComplete.Value = true;
                });
            }
            catch (Exception e)
            {
                Scheduler.Add(() =>
                {
                    errorMessage.Text = e.Message.Length > 100 ? e.Message.Substring(0, 100) + "..." : e.Message;
                    errorMessage.FadeIn(500, Easing.OutQuint);
                    loadingContainer.FadeOut(500, Easing.OutQuint);
                });
            }
        });
        thread.Start();
    }
}
