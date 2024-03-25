﻿using System.Collections.Generic;
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
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens.Games;

public partial class FourChoiceGameScreen(Question question) : MicroGameScreen(question)
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

    private BindableBool answered = new BindableBool();
    private EndangerEdButton endButton;
    private EndangerEdButton skipButton;

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
                Font = EndangerEdFont.GetFont(size: 40)
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
                    gameSessionStore.StopwatchClock.Stop();
                    onChoiceSelected("");
                }
            },
            new FillFlowContainer()
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Direction = FillDirection.Horizontal,
                Margin = new MarginPadding(10),
                Spacing = new Vector2(10),
                Height = 10,
                Alpha = question.QuestionMode == QuestionMode.FourChoice ? 0 : 1,
                Children = new Drawable[]
                {
                    new SpriteIcon()
                    {
                        Size = new Vector2(20),
                        Icon = FontAwesome.Solid.ExclamationTriangle,
                        Colour = Colour4.Yellow
                    },
                    new EndangerEdSpriteText()
                    {
                        Text = "Question mode " + question.QuestionMode.ToString().ToUpper() + " is not currently supported, falling back to default mode.",
                        Font = EndangerEdFont.GetFont(size: 20),
                        Colour = Colour4.Yellow
                    }
                }
            }
        };

        if (CurrentQuestion.ContentType == ContentType.Image)
        {
            AddInternal(new GridContainer()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new osuTK.Vector2(400, 400),
                Content = new[]
                {
                    [
                        new OnlineImageButton(CurrentQuestion.Choices[0])
                        {
                            Size = new osuTK.Vector2(200, 200),
                            Action = () => onChoiceSelected(CurrentQuestion.Choices[0])
                        },
                        new OnlineImageButton(CurrentQuestion.Choices[1])
                        {
                            Size = new osuTK.Vector2(200, 200),
                            Action = () => onChoiceSelected(CurrentQuestion.Choices[1])
                        }
                    ],
                    new Drawable[]
                    {
                        new OnlineImageButton(CurrentQuestion.Choices[2])
                        {
                            Size = new osuTK.Vector2(200, 200),
                            Action = () => onChoiceSelected(CurrentQuestion.Choices[2])
                        },
                        new OnlineImageButton(CurrentQuestion.Choices[3])
                        {
                            Size = new osuTK.Vector2(200, 200),
                            Action = () => onChoiceSelected(CurrentQuestion.Choices[3])
                        }
                    }
                }
            });
        }
        else
        {
            AddInternal(new FillFlowContainer()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new osuTK.Vector2(0, 10),
                Children = new Drawable[]
                {
                    new EndangerEdButton(CurrentQuestion.Choices[0])
                    {
                        Size = new osuTK.Vector2(200, 50),
                        Action = () => onChoiceSelected(CurrentQuestion.Choices[0])
                    },
                    new EndangerEdButton(CurrentQuestion.Choices[1])
                    {
                        Size = new osuTK.Vector2(200, 50),
                        Action = () => onChoiceSelected(CurrentQuestion.Choices[1])
                    },
                    new EndangerEdButton(CurrentQuestion.Choices[2])
                    {
                        Size = new osuTK.Vector2(200, 50),
                        Action = () => onChoiceSelected(CurrentQuestion.Choices[2])
                    },
                    new EndangerEdButton(CurrentQuestion.Choices[3])
                    {
                        Size = new osuTK.Vector2(200, 50),
                        Action = () => onChoiceSelected(CurrentQuestion.Choices[3])
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
