using EndangerEd.Game.Graphics;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace EndangerEd.Game.Screens.Games;

public partial class GameOverScreen : EndangerEdScreen
{
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
}
