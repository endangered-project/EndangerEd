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
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osuTK;

namespace EndangerEd.Game.Screens.Games;

/// <summary>
/// Game about receive a falling answer to answer a question.
/// </summary>
/// <param name="question"></param>
public partial class BucketGameScreen(Question question) : MicroGameScreen(question)
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

    private Sprite bucket;

    private bool allowMovingBucket = true;

    private Sample bucketImpactSample;
    private Sample correctAnswerSample;
    private Sample incorrectAnswerSample;

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, TextureStore textureStore)
    {
        bucketImpactSample = audioManager.Samples.Get($"Game/Bucket/BucketImpact{RNG.Next(0, 3)}.wav");
        correctAnswerSample = audioManager.Samples.Get("UI/CorrectNotify.wav");
        incorrectAnswerSample = audioManager.Samples.Get("UI/WrongNotify.wav");

        if (CurrentQuestion.ContentType == ContentType.Image)
        {
            AddInternal(boxContainer1 = new Container()
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                // Limit the position of PNG to make the box still in the screen.
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                // Limit the position of PNG to make the box still in the screen.
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(10, 90) * 0.01f, -0.36f),
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
        
        AddInternal(new EndangerEdSpriteText()
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Text = CurrentQuestion.QuestionText,
            Font = EndangerEdFont.GetFont(size: 40, weight: EndangerEdFont.FontWeight.Bold)
        });
        AddInternal(new EndangerEdSpriteText()
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Text = "Move the bucket to catch the answer!",
            Position = new Vector2(0, 50),
            Font = EndangerEdFont.GetFont(size: 30)
        });
        AddInternal(bucket = new Sprite()
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Scale = new Vector2(0.4f, 0.4f),
            Texture = textureStore.Get("Game/Bucket/Bucket.png")
        });

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

                        try
                        {
                            if (!gameSessionStore.IsDefaultGame())
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
                    allowMovingBucket = false;
                    gameSessionStore.StopwatchClock.Stop();
                    onChoiceSelected("");
                }
            }
        });
    }

    private bool hasPlayedBucketImpactSample;

    private void playBucketImpactSample()
    {
        if (!hasPlayedBucketImpactSample)
        {
            bucketImpactSample?.Play();
            hasPlayedBucketImpactSample = true;
        }
    }

    protected override void Update()
    {
        if (gameSessionStore.IsOverTime() && !IsOverTime)
        {
            IsOverTime = true;
            onChoiceSelected("");
        }

        if (bucket.Contains(boxContainer1.ScreenSpaceDrawQuad.Centre))
        {
            boxContainer1.FlashColour(Colour4.White, 500);
            bucket.MoveToOffset(new Vector2(0, 20), 100).Then().MoveToOffset(new Vector2(0, -20), 100);
            allowMovingBucket = false;
            playBucketImpactSample();
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[0]);
        }

        if (bucket.Contains(boxContainer2.ScreenSpaceDrawQuad.Centre))
        {
            boxContainer2.FlashColour(Colour4.White, 500);
            bucket.MoveToOffset(new Vector2(0, 20), 100).Then().MoveToOffset(new Vector2(0, -20), 100);
            allowMovingBucket = false;
            playBucketImpactSample();
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[1]);
        }

        if (bucket.Contains(boxContainer3.ScreenSpaceDrawQuad.Centre))
        {
            boxContainer3.FlashColour(Colour4.White, 500);
            bucket.MoveToOffset(new Vector2(0, 20), 100).Then().MoveToOffset(new Vector2(0, -20), 100);
            allowMovingBucket = false;
            playBucketImpactSample();
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[2]);
        }

        if (bucket.Contains(boxContainer4.ScreenSpaceDrawQuad.Centre))
        {
            boxContainer4.FlashColour(Colour4.White, 500);
            bucket.MoveToOffset(new Vector2(0, 20), 100).Then().MoveToOffset(new Vector2(0, -20), 100);
            allowMovingBucket = false;
            playBucketImpactSample();
            stopBoxContainer();
            onChoiceSelected(CurrentQuestion.Choices[3]);
        }

        // If all bucket is not in the screen, then the game is over.
        if (boxContainer1.Position.Y > 1.2f && boxContainer2.Position.Y > 1.2f && boxContainer3.Position.Y > 1.2f && boxContainer4.Position.Y > 1.2f)
        {
            allowMovingBucket = false;
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
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        // Move the bucket to the mouse position.
        if (allowMovingBucket)
            bucket.MoveTo(new Vector2(e.MousePosition.X - bucket.AnchorPosition.X, bucket.Position.Y), 1000, Easing.OutExpo);
        return base.OnMouseMove(e);
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
            if (allowMovingBucket)
                boxContainer1.MoveTo(new Vector2(boxContainer1.Position.X, 1.3f), 3000, Easing.InExpo);
        }, 0);
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
                boxContainer2.MoveTo(new Vector2(boxContainer2.Position.X, 1.3f), 3000, Easing.InExpo);
        }, 3000);
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
                boxContainer3.MoveTo(new Vector2(boxContainer3.Position.X, 1.3f), 3000, Easing.InExpo);
        }, 6000);
        Scheduler.AddDelayed(() =>
        {
            if (allowMovingBucket)
                boxContainer4.MoveTo(new Vector2(boxContainer4.Position.X, 1.3f), 3000, Easing.InExpo);
        }, 12000);
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
