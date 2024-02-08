using osu.Framework.Graphics.Sprites;

namespace EndangerEd.Game.Graphics;

/// <summary>
/// The <see cref="SpriteText"/> that initialized with default font.
/// </summary>
public partial class EndangerEdSpriteText : SpriteText
{
    public EndangerEdSpriteText()
    {
        Font = EndangerEdFont.GetFont();
    }
}
