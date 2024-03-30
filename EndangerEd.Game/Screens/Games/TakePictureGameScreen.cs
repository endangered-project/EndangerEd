using System;
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
/// Game about taking picture of jumping fish.
/// </summary>
/// <param name="question"></param>
public partial class TakePictureGameScreen(Question question) : MicroGameScreen(question)
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

    private Container fishContainer1;
    private Container fishContainer2;
    private Container fishContainer3;
    private Container fishContainer4;

    private Container camera;
    private Sprite cameraBox;
    private Box cameraShutter;

    private bool allowMovingFish = true;

    private Sample cameraSample;
    private Sample splashSample;
    private Sample correctAnswerSample;
    private Sample incorrectAnswerSample;

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager, TextureStore textureStore)
    {
        cameraSample = audioManager.Samples.Get("Game/Camera/Camera.wav");
        splashSample = audioManager.Samples.Get("Game/Camera/Splash.wav");
        correctAnswerSample = audioManager.Samples.Get("UI/CorrectNotify.wav");
        incorrectAnswerSample = audioManager.Samples.Get("UI/WrongNotify.wav");

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
                Text = "Take a picture of the correct fish!",
                Position = new Vector2(0, 50),
                Font = EndangerEdFont.GetFont(size: 30)
            }
        };

        if (CurrentQuestion.ContentType == ContentType.Image)
        {
            AddInternal(fishContainer1 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                // Limit the position of PNG to make the box still in the screen.
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer2 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer3 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer4 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer1 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                // Limit the position of PNG to make the box still in the screen.
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer2 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer3 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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
            AddInternal(fishContainer4 = new Container()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(150, 150),
                RelativePositionAxes = Axes.Both,
                Position = new Vector2(RNG.Next(-40, 40) * 0.01f, 0.3f),
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

        AddInternal(camera = new Container()
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(200, 200),
            Children = new Drawable[]
            {
                cameraShutter = new Box()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.6f, 0.7f),
                    Position = new Vector2(0, 10),
                    Colour = Colour4.White,
                    Alpha = 0,
                    Blending = BlendingParameters.Additive
                },
                cameraBox = new Sprite()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(0.3f, 0.3f),
                    Position = new Vector2(0, -15),
                    Texture = textureStore.Get("Game/Camera/Camera.png")
                }
            }
        });

        // Water
        AddInternal(new Sprite()
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.Centre,
            Position = new Vector2(0, -10),
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(1.1f),
            FillMode = FillMode.Fit,
            Alpha = 0.9f,
            Texture = textureStore.Get("Game/Camera/Water.png")
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
                    allowMovingFish = false;
                    stopFishContainer();

                    Thread thread = new Thread(() =>
                    {
                        Scheduler.Add(() => sessionStore.IsLoading.Value = true);

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

        answered.BindValueChanged(answered =>
        {
            if (answered.NewValue)
            {
                endButton.Enabled.Value = false;
                skipButton.Enabled.Value = false;
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

    private void stopFishContainer()
    {
        fishContainer1.ClearTransforms();
        fishContainer2.ClearTransforms();
        fishContainer3.ClearTransforms();
        fishContainer4.ClearTransforms();
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        // Move the camera to the mouse position.
        if (allowMovingFish)
            camera.Position = new Vector2(e.MousePosition.X - DrawSize.X / 2, e.MousePosition.Y - DrawSize.Y / 2 < 0 ? e.MousePosition.Y - DrawSize.Y / 2 : 0);
        return base.OnMouseMove(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        cameraShutter.FadeTo(0.75f, 100).Then().FadeOut(100);

        if (camera.Contains(fishContainer1.ScreenSpaceDrawQuad.Centre))
        {
            allowMovingFish = false;
            stopFishContainer();
            onChoiceSelected(CurrentQuestion.Choices[0]);
        }

        if (camera.Contains(fishContainer2.ScreenSpaceDrawQuad.Centre))
        {
            allowMovingFish = false;
            stopFishContainer();
            onChoiceSelected(CurrentQuestion.Choices[1]);
        }

        if (camera.Contains(fishContainer3.ScreenSpaceDrawQuad.Centre))
        {
            allowMovingFish = false;
            stopFishContainer();
            onChoiceSelected(CurrentQuestion.Choices[2]);
        }

        if (camera.Contains(fishContainer4.ScreenSpaceDrawQuad.Centre))
        {
            allowMovingFish = false;
            stopFishContainer();
            onChoiceSelected(CurrentQuestion.Choices[3]);
        }

        cameraSample?.Play();

        return base.OnMouseDown(e);
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
        
        int duration = 3000 - gameSessionStore.Score.Value / 50 * 180;
        
        double nextJump = Clock.CurrentTime + duration * 4;

        spawnFishes(duration);
        duration = Math.Clamp(duration - 300, 1000, 3000);

        Scheduler.AddDelayed(() =>
        {
            if (Clock.CurrentTime < nextJump) return;
            
            duration = Math.Clamp(duration - 300, 1000, 3000);
            spawnFishes(duration);
            
            nextJump = Clock.CurrentTime + duration * 4;
        }, 0, true);
    }

    private void spawnFishes(int duration)
    { 
        Scheduler.AddDelayed(() =>
        {
            if (!allowMovingFish) return;

            float randomX = RNG.Next(-40, 40) * 0.01f;
            float jumpHeight = RNG.Next(-90, -50) * 0.01f;
            float jumpWidth = RNG.Next(0, 30) * 0.01f;

            fishContainer1.MoveTo(new Vector2(randomX, jumpHeight), duration / 2, Easing.OutCirc).Then().MoveTo(new Vector2(randomX + jumpWidth, 0.3f), duration / 2, Easing.InCirc);

            splashSample?.Play();
        }, 1000);

        Scheduler.AddDelayed(() =>
        {
            if (!allowMovingFish) return;

            float randomX = RNG.Next(-40, 40) * 0.01f;
            float jumpHeight = RNG.Next(-90, -50) * 0.01f;
            float jumpWidth = RNG.Next(0, 30) * 0.01f;

            fishContainer2.MoveTo(new Vector2(randomX, jumpHeight), duration / 2, Easing.OutCirc).Then().MoveTo(new Vector2(randomX + jumpWidth, 0.3f), duration / 2, Easing.InCirc);

            splashSample?.Play();
        }, duration + 1000);
        Scheduler.AddDelayed(() =>
        {
            if (!allowMovingFish) return;

            float randomX = RNG.Next(-40, 40) * 0.01f;
            float jumpHeight = RNG.Next(-90, -50) * 0.01f;
            float jumpWidth = RNG.Next(0, 30) * 0.01f;

            fishContainer3.MoveTo(new Vector2(randomX, jumpHeight), duration / 2, Easing.OutCirc).Then().MoveTo(new Vector2(randomX + jumpWidth, 0.3f), duration / 2, Easing.InCirc);

            splashSample?.Play();
        }, duration * 2 + 1000);

        Scheduler.AddDelayed(() =>
        {
            if (!allowMovingFish) return;

            float randomX = RNG.Next(-40, 40) * 0.01f;
            float jumpHeight = RNG.Next(-90, -50) * 0.01f;
            float jumpWidth = RNG.Next(0, 30) * 0.01f;

            fishContainer4.MoveTo(new Vector2(randomX, jumpHeight), duration / 2, Easing.OutCirc).Then().MoveTo(new Vector2(randomX + jumpWidth, 0.3f), duration / 2, Easing.InCirc);

            splashSample?.Play();
        }, duration * 3 + 1000);
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
