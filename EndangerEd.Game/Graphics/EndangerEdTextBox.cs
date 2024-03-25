using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics.UserInterface;

namespace EndangerEd.Game.Graphics;

public partial class EndangerEdTextBox : BasicTextBox
{
    protected virtual bool AllowUniqueCharacterSamples => true;

    protected override float LeftRightPadding => 10;

    private Sample hoverSample;
    private Sample clickSample;

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager)
    {
        hoverSample = audioManager.Samples.Get("UI/click-small-right.wav");
        clickSample = audioManager.Samples.Get("UI/click-small-left.wav");
    }

    protected override void OnTextSelectionChanged(TextSelectionType selectionType)
    {
        base.OnTextSelectionChanged(selectionType);

        switch (selectionType)
        {
            case TextSelectionType.Character:
                hoverSample.Play();
                break;

            case TextSelectionType.Word:
                hoverSample.Play();
                break;

            case TextSelectionType.All:
                clickSample.Play();
                break;
        }
    }

    protected override void OnUserTextAdded(string added)
    {
        base.OnUserTextAdded(added);

        if (AllowUniqueCharacterSamples)
            hoverSample.Play();
    }

    protected override void OnUserTextRemoved(string removed)
    {
        base.OnUserTextRemoved(removed);

        if (AllowUniqueCharacterSamples)
            clickSample.Play();
    }
}
