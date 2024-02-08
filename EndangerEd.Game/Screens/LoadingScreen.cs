using EndangerEd.Game.Graphics;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
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
    private EndangerEdMainScreenStack screenStack { get; set; }

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
                    Bottom = 130,
                    Right = 20
                },
                Action = this.Exit,
                Alpha = 0,
                Scale = new Vector2(2f)
            },
            loadingText = new EndangerEdSpriteText()
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Text = "Loading...",
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

        exitButton.FadeInFromZero(1000, Easing.OutQuint);
        Scheduler.AddDelayed(FinishLoading, 2000);
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
            screenStack.SwapScreenStack();
        });
    }
}
