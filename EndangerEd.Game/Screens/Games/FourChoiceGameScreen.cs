using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using EndangerEd.Game.API;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
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
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

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
            new FillFlowContainer()
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
            }
        };
    }

    protected override void Update()
    {
        if (gameSessionStore.IsOverTime() && !IsOverTime)
        {
            IsOverTime = true;
            onChoiceSelected("");
        }
    }

    private void onChoiceSelected(string choice)
    {
        gameSessionStore.StopwatchClock.Stop();

        if (choice == CurrentQuestion.Answer)
        {
            this.FlashColour(Colour4.Green, 500);
        }
        else
        {
            this.FlashColour(Colour4.Red, 500);
            gameSessionStore.Life.Value--;
        }

        if (gameSessionStore.Life.Value == 0)
        {
            Scheduler.Add(() =>
            {
                this.Exit();
                mainScreenStack.Push(new GameOverScreen());
            });
        }
        else
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    var result = apiRequestManager.PostJson("game/answer/", new Dictionary<string, object>
                    {
                        { "answer", choice }
                    });

                    // TODO: Make API return score
                    var score = result.TryGetValue("score", out var scoreValue) ? (int)scoreValue : 0;
                    gameSessionStore.Score.Value += score;
                }
                catch (HttpRequestException e)
                {
                    Logger.Log($"Request to game/answer failed with error: {e.Message}");
                }

                try
                {
                    var result = apiRequestManager.PostJson("game/question/", new Dictionary<string, object>());
                    var nextQuestion = new Question
                    {
                        QuestionText = result["rendered_question"].ToString(),
                        Choices = (string[])result["choices"],
                        Answer = result["answer"].ToString(),
                        ContentType = result["content_type"].ToString() == "image" ? ContentType.Image : ContentType.Text,
                        QuestionMode = result["question_mode"].ToString()
                    };

                    Scheduler.AddDelayed(() =>
                    {
                        mainScreenStack.PushQuestionScreen(nextQuestion);
                    }, 1000);

                    Scheduler.Add(this.Exit);
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
