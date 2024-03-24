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
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens;

/// <summary>
/// A screen for showing a loading screen on start playing.
/// </summary>
public partial class LoadingScreen : EndangerEdScreen
{
    private Box loadingBar;
    private const float loading_bar_height = 20;
    private Button exitButton;
    private SpriteText loadingText;

    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

    [Resolved]
    private AudioPlayer audioPlayer { get; set; }

    private BindableBool allowExit = new BindableBool(true);

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Box()
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                RelativeSizeAxes = Axes.X,
                Height = loading_bar_height,
                Colour = Colour4.Black,
                Alpha = 0.5f
            },
            loadingBar = new Box()
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.X,
                Height = loading_bar_height,
                Colour = Colour4.GreenYellow,
                Width = 0
            },
            exitButton = new EndangerEdButton("Exit")
            {
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomRight,
                Width = 150,
                Height = 40,
                Margin = new MarginPadding
                {
                    Bottom = 50,
                    Right = 20
                },
                Action = () => mainScreenStack.MainScreenStack.Push(new MainMenuScreen()),
                Alpha = 0
            },
            loadingText = new EndangerEdSpriteText()
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Text = "Loading...",
                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.MPlus1P, 30),
                Margin = new MarginPadding
                {
                    Bottom = 30,
                    Left = 20
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Thread thread = new Thread(() =>
        {
            Scheduler.Add(() => sessionStore.IsLoading.Value = true);

            try
            {
                var result = apiRequestManager.PostJson("game/start", new Dictionary<string, object>());
                gameSessionStore.GameId = int.Parse(result["game_id"].ToString());
                sessionStore.IsGameStarted.Value = true;
                Scheduler.Add(gameSessionStore.Reset);

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

                Scheduler.Add(FinishLoading);
            }
            catch (HttpRequestException e)
            {
                Scheduler.Add(() =>
                {
                    loadingBar.Colour = Colour4.Red;
                    loadingBar.ResizeTo(new Vector2(1, loading_bar_height), 1000, Easing.OutQuint);
                    loadingText.Text = "Failed to load : " + e.Message;
                    exitButton.FadeInFromZero(1000, Easing.OutQuint);
                });
            }
            catch (Exception e)
            {
                Scheduler.Add(() =>
                {
                    Logger.Error(e, "Failed to load game");
                    loadingBar.Colour = Colour4.Red;
                    loadingBar.ResizeTo(new Vector2(1, loading_bar_height), 1000, Easing.OutQuint);
                    loadingText.Text = "Failed to load : " + e.Message;
                    exitButton.FadeInFromZero(1000, Easing.OutQuint);
                });
            }

            Scheduler.Add(() => sessionStore.IsLoading.Value = false);
        });
        thread.Start();
    }

    /// <summary>
    /// Update the loading bar.
    /// </summary>
    /// <param name="progress">Progress of the loading bar.</param>
    public void UpdateLoadingBar(float progress)
    {
        loadingBar.ResizeTo(new Vector2(progress, loading_bar_height), 1000, Easing.OutQuint);
    }

    /// <summary>
    /// Finish loading and go to the next screen.
    /// </summary>
    public void FinishLoading()
    {
        UpdateLoadingBar(1);
        this.Delay(1000).Then().Schedule(() => { loadingText.Text = "Loading finished!"; }).Then().Delay(1000).Schedule(() =>
        {
            this.Exit();
            mainScreenStack.SwapScreenStack();
        });
    }
}
