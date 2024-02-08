using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace EndangerEd.Game.Screens.ScreenStacks;

public partial class EndangerEdMainScreenStack : ScreenStack
{
    [Resolved]
    private SessionStore sessionStore { get; set; }

    public EndangerEdGameSessionScreenStack GameScreenStack;

    public ScreenStack MainScreenStack;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new BackgroundScreen()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            },
            GameScreenStack = new EndangerEdGameSessionScreenStack
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Alpha = 0f
            },
            MainScreenStack = new ScreenStack
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            }
        };
    }

    public void SwapScreenStack()
    {
        if (GameScreenStack.Alpha != 0f)
        {
            GameScreenStack.Hide();
            MainScreenStack.Show();
        }
        else
        {
            GameScreenStack.Show();
            MainScreenStack.Hide();
        }
    }
}
