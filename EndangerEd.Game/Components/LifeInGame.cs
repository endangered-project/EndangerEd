using System.Linq;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace EndangerEd.Game.Components;

public partial class LifeInGame : CompositeDrawable
{
    private FillFlowContainer heartContainer;

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new SpriteText
            {
                Text = "Life",
                Font = new FontUsage(size: 30),
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Margin = new MarginPadding(10),
            },
            heartContainer = new FillFlowContainer
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Margin = new MarginPadding
                {
                    Top = 40,
                    Left = 10,
                },
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        gameSessionStore.Life.BindValueChanged(life =>
        {
            if (life.OldValue - life.NewValue == 1)
            {
                removeHeart();
            }
            else
            {
                updateLife(life.NewValue);
            }
        });
        updateLife(gameSessionStore.Life.Value);
    }

    /// <summary>
    /// Returns a new heart icon
    /// </summary>
    /// <returns>A new heart icon</returns>
    private SpriteIcon createHeart()
    {
        SpriteIcon heartIcon = new SpriteIcon
        {
            Icon = FontAwesome.Solid.Heart,
            Size = new Vector2(30),
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Colour = Colour4.Red,
            Margin = new MarginPadding
            {
                Right = 10
            }
        };

        return heartIcon;
    }

    /// <summary>
    /// Re-creates the heart icons based on the life value
    /// </summary>
    /// <param name="life">The life value</param>
    private void updateLife(int life)
    {
        heartContainer.Clear();

        for (int i = 0; i < gameSessionStore.Life.Value; i++)
        {
            heartContainer.Add(createHeart());
        }
    }

    /// <summary>
    /// Removes the last heart icon
    /// </summary>
    private void removeHeart()
    {
        if (heartContainer.Children.Count == 0)
        {
            return;
        }

        Drawable lastHeart = heartContainer.Children.Last();
        // Add fade out animation to the last heart
        Schedule(() => lastHeart.FlashColour(Colour4.White, 500, Easing.OutQuint).FadeOut(250).Expire());
    }
}
