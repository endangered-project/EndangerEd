using EndangerEd.Game.Graphics;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace EndangerEd.Game.Components;

public partial class ScoreDisplay : CompositeDrawable
{
    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    private SpriteText scoreText;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new SpriteText
            {
                Text = "SCORE",
                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 30f, EndangerEdFont.FontWeight.Bold),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Margin = new MarginPadding(10),
            },
            scoreText = new SpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = gameSessionStore.Score.Value.ToString(),
                Font = new FontUsage(size:30),
                Margin = new MarginPadding
                {
                    Top = 40,
                    Left = 10,
                },
            },
            new Timer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        gameSessionStore.Score.BindValueChanged(score =>
        {
            // Add comma separator
            scoreText.Text = score.NewValue.ToString("N0");
        });
    }
}
