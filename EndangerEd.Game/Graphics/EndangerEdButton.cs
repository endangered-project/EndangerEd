using EndangerEd.Game.Components;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace EndangerEd.Game.Graphics;

public partial class EndangerEdButton : Button
{
    private string text;
    public Colour4 ButtonColour = Colour4.Green;
    private Box buttonBox;
    private SpriteText buttonText;
    private Box lockMask;

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
                Masking = true,
                CornerRadius = 20,
                BorderColour = Colour4.White,
                BorderThickness = 5,
                Children = new Drawable[]
                {
                    buttonBox = new Box()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = ButtonColour
                    },
                    buttonText = new SpriteText()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = text,
                    }
                },
            },
            new ClickHoverSounds(Enabled),
            new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 20,
                Children = new Drawable[]
                {
                    lockMask = new Box()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0
                    }
                }
            }
        };

        Enabled.BindValueChanged(enabled =>
        {
            if (!enabled.NewValue)
                lockMask.FadeTo(0.5f, 200);
            else
                lockMask.FadeOut(200);
        }, true);
    }

    public EndangerEdButton(string text)
    {
        this.text = text;
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (Enabled.Value)
            buttonBox.Colour = ButtonColour.Darken(0.25f);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (Enabled.Value)
            buttonBox.Colour = ButtonColour;
        base.OnHoverLost(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button == MouseButton.Left && Enabled.Value)
        {
            buttonBox.Colour = ButtonColour.Darken(0.5f);
            this.ScaleTo(0.9f, 100, Easing.Out);
            return true;
        }

        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button == MouseButton.Left && Enabled.Value)
        {
            buttonBox.Colour = ButtonColour.Darken(0.25f);
            this.ScaleTo(1, 1000, Easing.OutElastic);
        }

        base.OnMouseUp(e);
    }

    /// <summary>
    /// Sets the text of the button.
    /// </summary>
    /// <param name="text">A string containing the text to set.</param>
    public void SetText(string text)
    {
        buttonText.Text = text;
        this.text = text;
    }

    public void SetColour(Colour4 colour)
    {
        ButtonColour = colour;
        buttonBox.Colour = colour;
    }
}
