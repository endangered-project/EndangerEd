using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace EndangerEd.Game.Components;

public partial class MicrogameTutorialContainer : Container
{
    public IconUsage Icon;
    public new string Name;
    public string Description;
    public Question Question;

    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.TopLeft;
        Origin = Anchor.TopLeft;
        RelativeSizeAxes = Axes.X;
        Size = new Vector2(0.3f, 200);
        Masking = true;
        CornerRadius = 16;
        Children = new Drawable[]
        {
            new Box()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Colour = Color4Extensions.FromHex("6f6f75")
            },
            new FillFlowContainer()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Padding = new MarginPadding(15),
                Children = new Drawable[]
                {
                    new Container()
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1, 50),
                        Child = new SpriteIcon()
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = new Vector2(30),
                            Icon = Icon
                        }
                    },
                    new EndangerEdSpriteText()
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1, 20),
                        Text = Name,
                        Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 24f, EndangerEdFont.FontWeight.Bold)
                    },
                    new EndangerEdSpriteText()
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1, 40),
                        Text = Description,
                        Font = EndangerEdFont.GetFont()
                    },
                    new EndangerEdButton("Try it!")
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1, 40),
                        Action = () =>
                        {
                            gameSessionStore.Reset();
                            Scheduler.Add(() =>
                            {
                                mainScreenStack.PushQuestionScreen(Question);
                                mainScreenStack.SwapScreenStack();
                            });
                        }
                    }
                }
            }
        };
    }
}
