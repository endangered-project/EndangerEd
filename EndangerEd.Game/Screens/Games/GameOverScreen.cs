using EndangerEd.Game.Graphics;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace EndangerEd.Game.Screens.Games;

public partial class GameOverScreen : EndangerEdScreen
{
    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new EndangerEdSpriteText
            {
                Text = "Game Over",
                Font = EndangerEdFont.GetFont(size: 40),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Scheduler.AddDelayed(() =>
        {
            mainScreenStack.SwapScreenStack(100);
            mainScreenStack.MainScreenStack.Push(new ResultScreen(gameSessionStore.GameId));
        }, 3000);
    }
}
