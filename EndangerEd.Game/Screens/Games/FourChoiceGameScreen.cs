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
using osu.Framework.Logging;
using osu.Framework.Screens;

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
            }
        };

        if (question.ContentType == ContentType.Image)
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

        if (choice == CurrentQuestion.Answer)
        {
            this.FlashColour(Colour4.Green, 500);
        }
        else
        {
            this.FlashColour(Colour4.Red, 500);
            gameSessionStore.Life.Value--;
        }

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

        if (gameSessionStore.Life.Value == 0)
        {
            Scheduler.AddDelayed(() =>
            {
                this.Exit();
                mainScreenStack.GameScreenStack.MainScreenStack.Push(new GameOverScreen());
            }, 1000);
        }
        else
        {
            Thread thread = new Thread(() =>
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
                    }, 1000);
                }
                catch (HttpRequestException e)
                {
                    Logger.Log($"Request to game/question failed with error: {e.Message}");
                }
            });
            thread.Start();
        }
    }
}
