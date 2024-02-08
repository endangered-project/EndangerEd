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
    public Colour4 ButtonColour = Colour4.DarkGreen;
    private Box buttonBox;
    private SpriteText buttonText;

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
            new ClickHoverSounds()
        };
    }

    public EndangerEdButton(string text)
    {
        this.text = text;
    }

    protected override bool OnHover(HoverEvent e)
    {
        buttonBox.Colour = ButtonColour.Darken(0.25f);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        buttonBox.Colour = ButtonColour;
        base.OnHoverLost(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            buttonBox.Colour = ButtonColour.Darken(0.5f);
            return true;
        }

        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (e.Button == MouseButton.Left)
        {
            buttonBox.Colour = ButtonColour.Darken(0.25f);
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
