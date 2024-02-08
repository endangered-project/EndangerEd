using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace EndangerEd.Game.Screens;

public partial class BackgroundScreen : EndangerEdScreen
{
    private Sprite background;

    [BackgroundDependencyLoader]
    private void load(TextureStore store)
    {
        InternalChildren = new Drawable[]
        {
            background = new Sprite
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Stretch,
                RelativeSizeAxes = Axes.Both,
                Texture = store.Get("background.jpg")
            },
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0.6f
            }
        };
    }
}
