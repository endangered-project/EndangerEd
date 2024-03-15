using EndangerEd.Game.Components;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens.ScreenStacks;

public partial class EndangerEdGameSessionScreenStack : ScreenStack
{
    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    public ScreenStack MainScreenStack { get; set; }

    [BackgroundDependencyLoader]
    private void load(TextureStore store)
    {
        InternalChildren = new Drawable[]
        {
            new LifeInGame(),
            MainScreenStack = new ScreenStack
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.98f, 0.875f),
                Margin = new MarginPadding(10),
                Name = "Main screen"
            },
            new ScoreDisplay()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                RelativeSizeAxes = Axes.Both
            }
        };
    }
}
