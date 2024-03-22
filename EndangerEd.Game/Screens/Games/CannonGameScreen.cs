using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using EndangerEd.Game.API;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens.Games;

public partial class CannonGameScreen(Question question) : MicroGameScreen(question)
{
    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

    private BindableBool answered = new BindableBool();
    private bool allowFire = true;
    private EndangerEdButton endButton;
    private EndangerEdButton skipButton;

    private Container boxContainer1;
    private Container boxContainer2;
    private Container boxContainer3;
    private Container boxContainer4;

    private Box cannonBox;

    private double angle;

    private List<Circle> cannonBalls = new List<Circle>();

    [BackgroundDependencyLoader]
    private void load()
    {
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
                Text = "Fire the cannon to the correct answer!",
                Position = new Vector2(0, 50),
                Font = EndangerEdFont.GetFont(size: 30)
            },
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

                    Thread thread = new Thread(() =>
                    {
                        try
                        {
                            apiRequestManager.PostJson("game/end", new Dictionary<string, object>());
                            Scheduler.AddDelayed(() =>
                            {
                                mainScreenStack.SwapScreenStack(100);
                                mainScreenStack.MainScreenStack.Push(new ResultScreen(gameSessionStore.GameId));
                            }, 3000);
                        }
                        catch (HttpRequestException e)
                        {
                            Logger.Log($"Request to game/answer failed with error: {e.Message}");
                        }
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
            },
            cannonBox = new Box
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(80, 100),
                Colour = Colour4.Red,
                Depth = -10
            }
        };

        if (question.ContentType == ContentType.Image)
        {
            AddInternal(new FillFlowContainer()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Position = new Vector2(0, 200),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(100f),
                Children = new Drawable[]
                {
                    boxContainer1 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
                        Children = new Drawable[]
                        {
                            new Box()
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Red
                            },
                            new OnlineImageSprite(question.Choices[0])
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    },
                    boxContainer2 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
                        Children = new Drawable[]
                        {
                            new Box()
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Red
                            },
                            new OnlineImageSprite(question.Choices[1])
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    },
                    boxContainer3 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
                        Children = new Drawable[]
                        {
                            new Box()
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Red
                            },
                            new OnlineImageSprite(question.Choices[2])
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    },
                    boxContainer4 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
                        Children = new Drawable[]
                        {
                            new Box()
                            {
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Red
                            },
                            new OnlineImageSprite(question.Choices[3])
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    }
                }
            });
        }
        else
        {
            AddInternal(new FillFlowContainer()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Position = new Vector2(0, 200),
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(100f),
                Children = new Drawable[]
                {
                    boxContainer1 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
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
                                Text = question.Choices[0]
                            }
                        }
                    },
                    boxContainer2 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
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
                                Text = question.Choices[1]
                            }
                        }
                    },
                    boxContainer3 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
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
                                Text = question.Choices[2]
                            }
                        }
                    },
                    boxContainer4 = new Container()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(150, 150),
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
                                Text = question.Choices[3]
                            }
                        }
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
    }

    protected override void Update()
    {
        if (gameSessionStore.IsOverTime() && !IsOverTime)
        {
            IsOverTime = true;
            onChoiceSelected("");
        }

        foreach (Circle cannonBall in cannonBalls)
        {
            // Check cannon collision
            if (boxContainer1.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopLeft) || boxContainer1.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopRight) || boxContainer1.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomLeft) || boxContainer1.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomRight))
            {
                boxContainer1.FlashColour(Colour4.White, 500);
                stopAllBullet();
                allowFire = false;
                onChoiceSelected(question.Choices[0]);
            }
            else if (boxContainer2.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopLeft) || boxContainer2.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopRight) || boxContainer2.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomLeft) || boxContainer2.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomRight))
            {
                boxContainer2.FlashColour(Colour4.White, 500);
                stopAllBullet();
                allowFire = false;
                onChoiceSelected(question.Choices[1]);
            }
            else if (boxContainer3.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopLeft) || boxContainer3.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopRight) || boxContainer3.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomLeft) || boxContainer3.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomRight))
            {
                boxContainer3.FlashColour(Colour4.White, 500);
                stopAllBullet();
                allowFire = false;
                onChoiceSelected(question.Choices[2]);
            }
            else if (boxContainer4.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopLeft) || boxContainer4.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.TopRight) || boxContainer4.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomLeft) || boxContainer4.ScreenSpaceDrawQuad.Contains(cannonBall.ScreenSpaceDrawQuad.BottomRight))
            {
                boxContainer4.FlashColour(Colour4.White, 500);
                stopAllBullet();
                allowFire = false;
                onChoiceSelected(question.Choices[3]);
            }
        }
    }

    private void stopAllBullet()
    {
        foreach (Circle cannonBall in cannonBalls)
        {
            cannonBall.ClearTransforms();
        }
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (!allowFire)
            return base.OnMouseMove(e);

        // Rotate the cannon using the mouse position
        // Compute the angle between the cannon and the mouse position
        // Angle will output between -1.5 to 0, normalize number to -60 to 60
        angle = -Math.Atan2(e.MousePosition.Y - cannonBox.Position.Y, e.MousePosition.X - cannonBox.Position.X);
        // Normalize the angle
        angle = angle * 180 / Math.PI * 1.25 + 60;
        Logger.Log($"Angle: {angle}");
        cannonBox.Rotation = (float)angle;
        return base.OnMouseMove(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (!allowFire)
            return base.OnMouseDown(e);

        // Summon the cannon ball
        Circle cannonBall = new Circle
        {
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            RelativePositionAxes = Axes.Both,
            Size = new Vector2(30, 30),
            Colour = Colour4.Yellow,
            Position = cannonBox.Position
        };
        cannonBalls.Add(cannonBall);
        AddInternal(cannonBall);
        // Calculate target position using pythagorean theorem
        // Lock y position to 1.3f
        // Calculate x position using angle
        double x = Math.Sin(angle * Math.PI / 180) * 1.3f;
        Logger.Log($"X: {x}");
        cannonBall.MoveTo(new Vector2((float)x, -1.5f), 1000, Easing.OutQuint);
        return base.OnMouseDown(e);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        Scheduler.Add(() =>
        {
            gameSessionStore.StopwatchClock.Reset();
            gameSessionStore.StopwatchClock.Start();
        });
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
                var result = apiRequestManager.PostJson("game/answer", new Dictionary<string, object>
                {
                    { "answer", choice }
                });
                result.TryGetValue("score", out var scoreValue);
                gameSessionStore.Score.Value += scoreValue != null ? int.Parse(scoreValue.ToString()) : 0;
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

                    if (question.ContentType == ContentType.Image)
                    {
                        resultDetail.Add(new OnlineImageSprite(question.Answer)
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
                            Text = question.Answer,
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
                        mainScreenStack.PushQuestionScreen(nextQuestion);
                    }, 3000);
                }
                catch (HttpRequestException e)
                {
                    Logger.Log($"Request to game/question failed with error: {e.Message}");
                }
            }
        });
        thread.Start();
    }
}
