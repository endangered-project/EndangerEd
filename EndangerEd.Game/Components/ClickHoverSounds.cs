using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace EndangerEd.Game.Components;

/// <summary>
/// A <see cref="Drawable"/> that make a sound when hovered or clicked.
/// </summary>
public partial class ClickHoverSounds : Drawable
{
    private Sample hoverSample;
    private Sample clickSample;
    private readonly string hoverSampleName;
    private readonly string clickSampleName;

    public ClickHoverSounds(string hoverSampleName = "UI/hover.wav", string clickSampleName = "UI/click-enter1.wav")
    {
        this.hoverSampleName = hoverSampleName;
        this.clickSampleName = clickSampleName;
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager)
    {
        hoverSample = audioManager.Samples.Get(hoverSampleName);
        clickSample = audioManager.Samples.Get(clickSampleName);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hoverSample?.Play();
        return base.OnHover(e);
    }

    protected override bool OnClick(ClickEvent e)
    {
        clickSample?.Play();
        return base.OnClick(e);
    }
}
