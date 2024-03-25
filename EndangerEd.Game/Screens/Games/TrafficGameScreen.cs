using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using EndangerEd.Game.API;
using EndangerEd.Game.Audio;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens.Games;

/// <summary>
/// Game about release the car from the traffic jam.
/// </summary>
/// <param name="question"></param>
public partial class TrafficGameScreen(Question question) : MicroGameScreen(question)
{
    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

    [Resolved]
    private AudioPlayer audioPlayer { get; set; }

    private readonly BindableBool answered = new BindableBool();
    private EndangerEdButton endButton;
    private EndangerEdButton skipButton;

    private Container boxContainer1;
    private Container boxContainer2;
    private Container boxContainer3;
    private Container boxContainer4;

    private Button buttonChoice1;
    private SpriteIcon buttonSprite1;
    private Button buttonChoice2;
    private SpriteIcon buttonSprite2;
    private Button buttonChoice3;
    private SpriteIcon buttonSprite3;
    private Button buttonChoice4;
    private SpriteIcon buttonSprite4;

    private Box mainBarrier;

    private bool allowMovingCar = true;

    private Sample correctAnswerSample;
    private Sample incorrectAnswerSample;

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, TextureStore textureStore)
    {
        correctAnswerSample = audioManager.Samples.Get("UI/CorrectNotify.wav");
        incorrectAnswerSample = audioManager.Samples.Get("UI/WrongNotify.wav");

        InternalChildren = new Drawable[]
        {
            // Road
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.X,
                Size = new Vector2(0.5f, 300),
                Colour = Colour4.Gray
            },
            // Road for choice 1
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.Y,
                Size = new Vector2(125, 0.5f),
                Colour = Colour4.Gray
            },
            // Road for choice 2
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.X,
                Size = new Vector2(0.5f, 125),
                Colour = Colour4.Gray,
                Position = new Vector2(0, -75f)
            },
            // Road for choice 3
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.X,
                Size = new Vector2(0.5f, 125),
                Colour = Colour4.Gray,
                Position = new Vector2(0, 75f)
            },
            // Road for choice 4
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.TopCentre,
                RelativeSizeAxes = Axes.Y,
                Size = new Vector2(125, 0.5f),
                Colour = Colour4.Gray
            },
            // Choice 1 barrier
            buttonChoice1 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 25),
                Position = new Vector2(0, -150),
                Child = buttonSprite1 = new SpriteIcon()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.TrafficLight,
                    Colour = Colour4.Red,
                    Size = new Vector2(50)
                },
                Action = onReleaseChoice1,
                Enabled = { Value = false }
            },
            // Choice 2 barrier
            buttonChoice2 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(25, 125),
                Position = new Vector2(-75, -75),
                Child = buttonSprite2 = new SpriteIcon()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.TrafficLight,
                    Colour = Colour4.Red,
                    Size = new Vector2(50)
                },
                Action = onReleaseChoice2,
                Enabled = { Value = false }
            },
            // Choice 3 barrier
            buttonChoice3 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(25, 125),
                Position = new Vector2(-75, 75),
                Child = buttonSprite3 = new SpriteIcon()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.TrafficLight,
                    Colour = Colour4.Red,
                    Size = new Vector2(50)
                },
                Action = onReleaseChoice3,
                Enabled = { Value = false }
            },
            // Choice 4 barrier
            buttonChoice4 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 25),
                Position = new Vector2(0, 150),
                Child = buttonSprite4 = new SpriteIcon()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Icon = FontAwesome.Solid.TrafficLight,
                    Colour = Colour4.Red,
                    Size = new Vector2(50)
                },
                Action = onReleaseChoice4,
                Enabled = { Value = false }
            },
            // Main barrier
            mainBarrier = new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(25, 300),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0.4f, 0),
                Colour = Colour4.Red
            }
        };

        if (CurrentQuestion.ContentType == ContentType.Image)
        {
            AddInternal(boxContainer1 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0, -0.8f),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new OnlineImageSprite(CurrentQuestion.Choices[0])
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    }
                }
            });
            AddInternal(boxContainer2 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.X,
                Position = new Vector2(-0.8f, -75),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new OnlineImageSprite(CurrentQuestion.Choices[1])
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    }
                }
            });
            AddInternal(boxContainer3 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.X,
                Position = new Vector2(-0.8f, 75),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new OnlineImageSprite(CurrentQuestion.Choices[2])
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    }
                }
            });
            AddInternal(boxContainer4 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0, 0.8f),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new OnlineImageSprite(CurrentQuestion.Choices[3])
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    }
                }
            });
        }
        else
        {
            AddInternal(boxContainer1 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0, -0.8f),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new SpriteText()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = CurrentQuestion.Choices[0]
                    }
                }
            });
            AddInternal(boxContainer2 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.X,
                Position = new Vector2(-0.8f, -75),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new SpriteText()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = CurrentQuestion.Choices[1]
                    }
                }
            });
            AddInternal(boxContainer3 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.X,
                Position = new Vector2(-0.8f, 75),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new SpriteText()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = CurrentQuestion.Choices[2]
                    }
                }
            });
            AddInternal(boxContainer4 = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(125, 125),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(0, 0.8f),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Red
                    },
                    new SpriteText()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = CurrentQuestion.Choices[3]
                    }
                }
            });
        }

        answered.BindValueChanged(answered =>
        {
            if (answered.NewValue)
            {
                endButton.Enabled.Value = false;
                skipButton.Enabled.Value = false;
            }
        });

        AddRangeInternal(new Drawable[]
        {
            endButton = new EndangerEdButton("End")
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Margin = new MarginPadding(10),
                Width = 80,
                Height = 50,
                Action = () =>
                {
                    sessionStore.IsGameStarted.Value = false;
                    gameSessionStore.StopwatchClock.Stop();
                    answered.Value = true;
                    allowMovingCar = false;
                    stopBoxContainer();

                    Thread thread = new Thread(() =>
                    {
                        Scheduler.Add(() => sessionStore.IsLoading.Value = true);

                        try
                        {
                            if (gameSessionStore.IsDefaultGame())
                            {
                                apiRequestManager.PostJson("game/end", new Dictionary<string, object>());
                                Scheduler.AddDelayed(() =>
                                {
                                    mainScreenStack.SwapScreenStack(100);
                                    mainScreenStack.MainScreenStack.Push(new ResultScreen(gameSessionStore.GameId));
                                }, 3000);
                            }
                            else
                            {
                                Scheduler.AddDelayed(() =>
                                {
                                    mainScreenStack.SwapScreenStack(100);
                                }, 3000);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            Logger.Log($"Request to game/answer failed with error: {e.Message}");
                        }

                        Scheduler.Add(() => sessionStore.IsLoading.Value = false);
                    });
                    thread.Start();
                }
            },
            skipButton = new EndangerEdButton("Skip")
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Margin = new MarginPadding
                {
                    Bottom = 70,
                    Right = 10
                },
                Width = 80,
                Height = 50,
                Action = () =>
                {
                    stopBoxContainer();
                    allowMovingCar = false;
                    gameSessionStore.StopwatchClock.Stop();
                    onChoiceSelected("");
                }
            },
            new EndangerEdSpriteText()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = CurrentQuestion.QuestionText,
                Font = EndangerEdFont.GetFont(size: 40, weight: EndangerEdFont.FontWeight.Bold)
            },
            new EndangerEdSpriteText()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "Release the correct car from the traffic jam!",
                Position = new Vector2(0, 50),
                Font = EndangerEdFont.GetFont(size: 30)
            }
        });
    }

    protected override void Update()
    {
        if (gameSessionStore.IsOverTime() && !IsOverTime)
        {
            IsOverTime = true;
            onChoiceSelected("");
        }
    }

    private void stopBoxContainer()
    {
        boxContainer1.ClearTransforms();
        boxContainer2.ClearTransforms();
        boxContainer3.ClearTransforms();
        boxContainer4.ClearTransforms();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        audioPlayer.ChangeTrack("ingame.mp3");
        Scheduler.Add(() =>
        {
            gameSessionStore.StopwatchClock.Reset();
            gameSessionStore.StopwatchClock.Start();
        });

        Scheduler.AddDelayed(() =>
        {
            boxContainer1.MoveTo(new Vector2(0, -0.4f), 1000, Easing.OutQuint);
            boxContainer2.MoveTo(new Vector2(-0.2f, -75), 2000, Easing.OutQuint);
            boxContainer3.MoveTo(new Vector2(-0.2f, 75), 3000, Easing.OutQuint);
            boxContainer4.MoveTo(new Vector2(0, 0.4f), 4000, Easing.OutQuint);
        }, 1000);

        Scheduler.AddDelayed(() =>
        {
            buttonChoice1.Enabled.Value = true;
            buttonChoice2.Enabled.Value = true;
            buttonChoice3.Enabled.Value = true;
            buttonChoice4.Enabled.Value = true;
        }, 3000);
    }

    private void onReleaseChoice1()
    {
        if (!allowMovingCar) return;

        buttonSprite1.FlashColour(Colour4.White, 500).FadeColour(Colour4.LightGreen, 500);
        boxContainer1.MoveTo(new Vector2(0, 0f), 1000, Easing.OutQuint)
                     .Then()
                     .MoveTo(new Vector2(0.3f, 0), 1000, Easing.OutQuint);
        Scheduler.AddDelayed(() =>
        {
            if (question.Answer == question.Choices[0])
            {
                mainBarrier.FadeColour(Colour4.LightGreen, 500);
                boxContainer1.MoveTo(new Vector2(1f, 0), 1500, Easing.OutQuint);
            }
            else
            {
                mainBarrier.FlashColour(Colour4.White, 500);
            }
        }, 2000);
        Scheduler.AddDelayed(() =>
        {
            onChoiceSelected(CurrentQuestion.Choices[0]);
        }, 3000);
    }

    private void onReleaseChoice2()
    {
        if (!allowMovingCar) return;

        buttonSprite2.FlashColour(Colour4.White, 500).FadeColour(Colour4.LightGreen, 500);
        boxContainer2.MoveTo(new Vector2(0.05f, -75f), 500, Easing.OutQuint)
                     .Then()
                     .MoveTo(new Vector2(0.05f, 0), 500, Easing.OutQuint)
                     .Then()
                     .MoveTo(new Vector2(0.3f, 0), 500, Easing.OutQuint);
        Scheduler.AddDelayed(() =>
        {
            if (question.Answer == question.Choices[1])
            {
                mainBarrier.FadeColour(Colour4.LightGreen, 500);
                boxContainer2.MoveTo(new Vector2(1f, 0), 1500, Easing.OutQuint);
            }
            else
            {
                mainBarrier.FlashColour(Colour4.White, 500);
            }
        }, 2000);
        Scheduler.AddDelayed(() =>
        {
            onChoiceSelected(CurrentQuestion.Choices[1]);
        }, 3000);
    }

    private void onReleaseChoice3()
    {
        if (!allowMovingCar) return;

        buttonSprite3.FlashColour(Colour4.White, 500).FadeColour(Colour4.LightGreen, 500);
        boxContainer3.MoveTo(new Vector2(0.05f, 75f), 500, Easing.OutQuint)
                     .Then()
                     .MoveTo(new Vector2(0.05f, 0), 500, Easing.OutQuint)
                     .Then()
                     .MoveTo(new Vector2(0.3f, 0), 500, Easing.OutQuint);
        Scheduler.AddDelayed(() =>
        {
            if (question.Answer == question.Choices[2])
            {
                mainBarrier.FadeColour(Colour4.LightGreen, 500);
                boxContainer3.MoveTo(new Vector2(1f, 0), 1500, Easing.OutQuint);
            }
            else
            {
                mainBarrier.FlashColour(Colour4.White, 500);
            }
        }, 2000);
        Scheduler.AddDelayed(() =>
        {
            onChoiceSelected(CurrentQuestion.Choices[2]);
        }, 3000);
    }

    private void onReleaseChoice4()
    {
        if (!allowMovingCar) return;

        buttonSprite4.FlashColour(Colour4.White, 500).FadeColour(Colour4.LightGreen, 500);
        boxContainer4.MoveTo(new Vector2(0, 0f), 1000, Easing.OutQuint)
                     .Then()
                     .MoveTo(new Vector2(0.3f, 0), 1000, Easing.OutQuint);
        Scheduler.AddDelayed(() =>
        {
            if (question.Answer == question.Choices[3])
            {
                mainBarrier.FadeColour(Colour4.LightGreen, 500);
                boxContainer4.MoveTo(new Vector2(1f, 0), 1500, Easing.OutQuint);
            }
            else
            {
                mainBarrier.FlashColour(Colour4.White, 500);
            }
        }, 2000);
        Scheduler.AddDelayed(() =>
        {
            onChoiceSelected(CurrentQuestion.Choices[3]);
        }, 3000);
    }

    private void onChoiceSelected(string choice)
    {
        if (answered.Value)
            return;

        gameSessionStore.StopwatchClock.Stop();
        answered.Value = true;

        Thread thread = new Thread(() =>
        {
            try
            {
                if (gameSessionStore.IsDefaultGame())
                {
                    gameSessionStore.Score.Value += 50;
                }
                else
                {
                    var result = apiRequestManager.PostJson("game/answer", new Dictionary<string, object>
                    {
                        { "answer", choice }
                    });
                    result.TryGetValue("score", out var scoreValue);
                    gameSessionStore.Score.Value += scoreValue != null ? int.Parse(scoreValue.ToString()) : 0;
                }
            }
            catch (HttpRequestException e)
            {
                Logger.Log($"Request to game/answer failed with error: {e.Message}");
            }

            if (choice == CurrentQuestion.Answer)
            {
                Scheduler.Add(() =>
                {
                    this.FlashColour(Colour4.Green, 500);

                    correctAnswerSample?.Play();

                    Box loadingBox = new Box()
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 15,
                        Colour = Colour4.White,
                        Alpha = 0.75f
                    };

                    Container resultContainer = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(300, 300),
                        Masking = true,
                        CornerRadius = 20,
                        Scale = new Vector2(0),
                        Children = new Drawable[]
                        {
                            new Box()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = 0.75f
                            },
                            new FillFlowContainer()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(500, 500),
                                Direction = FillDirection.Vertical,
                                Masking = true,
                                Spacing = new Vector2(10),
                                Children = new Drawable[]
                                {
                                    new SpriteIcon()
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Icon = FontAwesome.Solid.CheckCircle,
                                        Size = new Vector2(100),
                                        Colour = Colour4.LightGreen
                                    },
                                    new EndangerEdSpriteText()
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Text = "Correct!".ToUpper(),
                                        Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 40, EndangerEdFont.FontWeight.Bold),
                                        Colour = Colour4.LightGreen
                                    }
                                }
                            },
                            loadingBox
                        }
                    };

                    AddInternal(resultContainer);
                    resultContainer.ScaleTo(1, 1000, Easing.OutElastic).Then().Delay(3000).ScaleTo(0, 1000, Easing.OutElastic);
                    loadingBox.ResizeWidthTo(0, 1500);
                });
            }
            else
            {
                gameSessionStore.Life.Value--;
                incorrectAnswerSample?.Play();
                Scheduler.Add(() =>
                {
                    this.FlashColour(Colour4.Red, 500);

                    Box loadingBox = new Box()
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 15,
                        Colour = Colour4.White,
                        Alpha = 0.75f
                    };

                    FillFlowContainer resultDetail = new FillFlowContainer()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(500, 500),
                        Direction = FillDirection.Vertical,
                        Masking = true,
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new SpriteIcon()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Icon = FontAwesome.Solid.TimesCircle,
                                Size = new Vector2(100),
                                Colour = Colour4.Red
                            },
                            new EndangerEdSpriteText()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Incorrect!".ToUpper(),
                                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 40, EndangerEdFont.FontWeight.Bold),
                                Colour = Colour4.Red
                            },
                            new EndangerEdSpriteText()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = $"The corrent answer is",
                                Font = EndangerEdFont.GetFont(size: 25),
                                Colour = Colour4.White
                            }
                        }
                    };

                    if (CurrentQuestion.ContentType == ContentType.Image)
                    {
                        resultDetail.Add(new OnlineImageSprite(CurrentQuestion.Answer)
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(150, 150)
                        });
                    }
                    else
                    {
                        resultDetail.Add(new EndangerEdSpriteText()
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Text = CurrentQuestion.Answer,
                            Font = EndangerEdFont.GetFont(size: 25),
                            Colour = Colour4.White
                        });
                    }

                    Container resultContainer = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(300, 400),
                        Masking = true,
                        CornerRadius = 20,
                        Scale = new Vector2(0),
                        Children = new Drawable[]
                        {
                            new Box()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = 0.75f
                            },
                            resultDetail,
                            loadingBox
                        }
                    };

                    AddInternal(resultContainer);
                    resultContainer.ScaleTo(1, 1000, Easing.OutElastic).Then().Delay(3000).ScaleTo(0, 1000, Easing.OutElastic);
                    loadingBox.ResizeWidthTo(0, 3000);
                });
            }

            if (gameSessionStore.IsDefaultGame())
            {
                Scheduler.AddDelayed(() =>
                {
                    this.Exit();
                    mainScreenStack.SwapScreenStack(100);
                }, 3000);
            }
            else
            {
                if (gameSessionStore.Life.Value == 0)
                {
                    Scheduler.AddDelayed(() =>
                    {
                        this.Exit();
                        mainScreenStack.GameScreenStack.MainScreenStack.Push(new GameOverScreen());
                    }, 3000);
                }
                else
                {
                    try
                    {
                        var questionResult = apiRequestManager.PostJson("game/question", new Dictionary<string, object>());
                        var jsonSerializer = JsonSerializer.Create();
                        var questionDict = jsonSerializer.Deserialize<Dictionary<string, object>>(new JsonTextReader(new StringReader(questionResult["question"].ToString())));
                        var gameModeDetail = jsonSerializer.Deserialize<Dictionary<string, object>>(new JsonTextReader(new StringReader(questionDict["game_mode"].ToString())));
                        var gameModeName = gameModeDetail["name"].ToString();
                        var nextQuestion = new Question
                        {
                            QuestionText = questionDict["rendered_question"].ToString(),
                            Choices = jsonSerializer.Deserialize<string[]>(new JsonTextReader(new StringReader(questionResult["choice"].ToString()))),
                            Answer = questionResult["answer"].ToString(),
                            ContentType = questionDict["type"].ToString() == "image" ? ContentType.Image : ContentType.Text,
                            QuestionMode = APIUtility.ConvertToQuestionMode(gameModeName)
                        };

                        Scheduler.AddDelayed(() =>
                        {
                            this.Exit();
                            mainScreenStack.PushQuestionScreen(nextQuestion);
                        }, 3000);
                    }
                    catch (HttpRequestException e)
                    {
                        Logger.Log($"Request to game/question failed with error: {e.Message}");
                    }
                }
            }
        });
        thread.Start();
    }
}
