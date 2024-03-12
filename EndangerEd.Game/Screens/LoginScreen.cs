using EndangerEd.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;

namespace EndangerEd.Game.Screens;

public partial class LoginScreen : EndangerEdScreen
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new osuTK.Vector2(0, 10),
                        Children = new Drawable[]
                        {
                            new EndangerEdSpriteText()
                            {
                                Text = "Login"
                            },
                            new EndangerEdButton("Login")
                            {
                                Action = () =>
                                {
                                    // Do login here
                                }
                            }
                        }
                    }
                }
            },
            new EndangerEdButton("Back")
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Margin = new MarginPadding(10),
                Action = this.Exit
            }
        };
    }
}
