using EndangerEd.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens;

public partial class TutorialScreen : EndangerEdScreen
{
    private SpriteText menuTitleSpriteText;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            menuTitleSpriteText = new EndangerEdSpriteText()
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Position = new Vector2(-600, 20),
                Text = "Tutorial".ToUpper(),
                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 48f, EndangerEdFont.FontWeight.Bold),
            }
        };

        AddInternal(new EndangerEdButton("Back")
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Size = new Vector2(100, 50),
            Position = new Vector2(20, -20),
            Action = this.Exit
        });
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        menuTitleSpriteText.MoveToX(20, 500, Easing.OutQuart);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        menuTitleSpriteText.MoveToX(-600, 500, Easing.OutQuart);
        return base.OnExiting(e);
    }
}
