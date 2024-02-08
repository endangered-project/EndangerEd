using EndangerEd.Game.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace EndangerEd.Game.Screens.ScreenStacks;

public partial class SettingsScreenStack : ScreenStack
{
    private SettingsContainer settingsContainer;

    public SettingsScreenStack()
    {
        InternalChild = settingsContainer = new SettingsContainer
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
        };
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        // ctrl + o to open settings.
        if (e.ControlPressed && e.Key == Key.O)
        {
            settingsContainer.ToggleVisibility();
        }

        return base.OnKeyDown(e);
    }

    /// <summary>
    /// Toggle visibility of settings container.
    /// </summary>
    public void ToggleVisibility()
    {
        settingsContainer.ToggleVisibility();
    }
}
