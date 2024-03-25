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
using osu.Framework.Audio.Track;
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
/// Game about put the thing out if not correct
/// </summary>
/// <param name="question"></param>
public partial class ConveyorGameScreen(Question question) : MicroGameScreen(question)
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

    private bool allowMovingBucket = true;

    private bool box1Removed;
    private bool box2Removed;
    private bool box3Removed;
    private bool box4Removed;

    private const float line_x_position = 0.3f;

    private Sample boxSpawnSample;
    private Sample boxRemoveSample;
    private Sample correctAnswerSample;
    private Sample incorrectAnswerSample;

    private Track conveyorLoopTrack;

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, TextureStore textureStore)
    {
        boxSpawnSample = audioManager.Samples.Get("Game/Conveyer/BoxSpawned.wav");
        boxRemoveSample = audioManager.Samples.Get("Game/Conveyer/BoxYeeted.wav");
        correctAnswerSample = audioManager.Samples.Get("UI/CorrectNotify.wav");
        incorrectAnswerSample = audioManager.Samples.Get("UI/WrongNotify.wav");

        conveyorLoopTrack = audioManager.Tracks.Get("conveyor-loop.mp3");
        conveyorLoopTrack.Looping = true;

        InternalChildren = new Drawable[]
        {
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
                Text = "Click the box that's not the answer out to remove it",
                Position = new Vector2(0, 50),
                Font = EndangerEdFont.GetFont(size: 30)
            },
            new EndangerEdSpriteText()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = "from the conveyor before it reach the barrier!",
                Position = new Vector2(0, 80),
                Font = EndangerEdFont.GetFont(size: 30)
            }
        };

        if (CurrentQuestion.ContentType == ContentType.Image)
        {
            AddInternal(boxContainer1 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                // Limit the position of PNG to make the box still in the screen.
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box1Removed) return;

                    boxContainer1.ClearTransforms();
                    boxContainer1.MoveTo(new Vector2(boxContainer1.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box1Removed = true;
                }
            });
            AddInternal(boxContainer2 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box2Removed) return;

                    boxContainer2.ClearTransforms();
                    boxContainer2.MoveTo(new Vector2(boxContainer2.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box2Removed = true;
                }
            });
            AddInternal(boxContainer3 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box3Removed) return;

                    boxContainer3.ClearTransforms();
                    boxContainer3.MoveTo(new Vector2(boxContainer3.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box3Removed = true;
                }
            });
            AddInternal(boxContainer4 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box4Removed) return;

                    boxContainer4.ClearTransforms();
                    boxContainer4.MoveTo(new Vector2(boxContainer4.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box4Removed = true;
                }
            });
        }
        else
        {
            AddInternal(boxContainer1 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box1Removed) return;

                    boxContainer1.ClearTransforms();
                    boxContainer1.MoveTo(new Vector2(boxContainer1.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box1Removed = true;
                }
            });
            AddInternal(boxContainer2 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box2Removed) return;

                    boxContainer2.ClearTransforms();
                    boxContainer2.MoveTo(new Vector2(boxContainer2.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box2Removed = true;
                }
            });
            AddInternal(boxContainer3 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box3Removed) return;

                    boxContainer3.ClearTransforms();
                    boxContainer3.MoveTo(new Vector2(boxContainer3.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box3Removed = true;
                }
            });
            AddInternal(boxContainer4 = new BasicButton()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(-0.6f, 0),
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
                },
                Action = () =>
                {
                    if (box4Removed) return;

                    boxContainer4.ClearTransforms();
                    boxContainer4.MoveTo(new Vector2(boxContainer4.Position.X, 1.3f), 500, Easing.InOutSine);
                    boxRemoveSample?.Play();
                    box4Removed = true;
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
                    allowMovingBucket = false;
                    stopBoxContainer();

                    Thread thread = new Thread(() =>
                    {
                        Scheduler.Add(() => sessionStore.IsLoading.Value = true);

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
                    gameSessionStore.StopwatchClock.Stop();
                    onChoiceSelected("");
                }
            }
        });

        // Conveyor belt
        AddInternal(new Sprite()
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Position = new Vector2(0, 110),
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.9f, 0.9f),
            FillMode = FillMode.Fill,
            Texture = textureStore.Get("Game/Conveyer/ConveyerBelt.png")
        });

        // Conveyor gear
        Sprite leftGear = new Sprite()
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.Centre,
            Position = new Vector2(50, 100),
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.12f, 0.12f),
            FillMode = FillMode.Fill,
            Texture = textureStore.Get("Game/Conveyer/Gear.png")
        };
        Sprite rightGear = new Sprite()
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.Centre,
            Position = new Vector2(-50, 100),
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.12f, 0.12f),
            FillMode = FillMode.Fill,
            Texture = textureStore.Get("Game/Conveyer/Gear.png")
        };

        AddInternal(leftGear);
        AddInternal(rightGear);

        leftGear.Spin(1000, RotationDirection.Clockwise).Loop();
        rightGear.Spin(1000, RotationDirection.Counterclockwise).Loop();
    }

    protected override void Update()
    {
        base.Update();

        if (boxContainer1.Position.X > line_x_position)
        {
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[0]);
        }

        if (boxContainer2.Position.X > line_x_position)
        {
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[1]);
        }

        if (boxContainer3.Position.X > line_x_position)
        {
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[2]);
        }

        if (boxContainer4.Position.X > line_x_position)
        {
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[3]);
        }

        // If all boxes are removed, stop the boxContainer.
        if (boxContainer1.Position.Y > 1f && boxContainer2.Position.Y > 1f && boxContainer3.Position.Y > 1f && boxContainer4.Position.Y > 1f)
        {
            stopBoxContainer();
            onChoiceSelected("");
        }
    }

    private void stopBoxContainer()
    {
        boxContainer1.ClearTransforms();
        boxContainer2.ClearTransforms();
        boxContainer3.ClearTransforms();
        boxContainer4.ClearTransforms();
        conveyorLoopTrack?.Stop();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        audioPlayer.ChangeTrack("ingame.mp3");
        conveyorLoopTrack?.Start();
        Scheduler.Add(() =>
        {
            gameSessionStore.StopwatchClock.Reset();
            gameSessionStore.StopwatchClock.Start();
        });

        // Add schedule to move the boxContainer to the bottom of the screen at the random time.
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
            {
                boxSpawnSample?.Play();
                boxContainer1.MoveTo(new Vector2(1.3f, boxContainer1.Position.Y), 6000);
            }
        }, 2000);
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
            {
                boxSpawnSample?.Play();
                boxContainer2.MoveTo(new Vector2(1.3f, boxContainer2.Position.Y), 6000);
            }
        }, 4000);
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
            {
                boxSpawnSample?.Play();
                boxContainer3.MoveTo(new Vector2(1.3f, boxContainer3.Position.Y), 6000);
            }
        }, 6000);
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
            {
                boxSpawnSample?.Play();
                boxContainer4.MoveTo(new Vector2(1.3f, boxContainer4.Position.Y), 6000);
            }
        }, 8000);
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
                            Size = new Vector2(100, 100)
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
