using EndangerEd.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace EndangerEd.Game.Screens.Games;

public partial class GameOverScreen : EndangerEdScreen
{
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
