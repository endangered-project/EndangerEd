using EndangerEd.Game.Graphics;
using osu.Framework.Graphics;
using osuTK;

namespace EndangerEd.Game.Tests.Visual.Components;

public partial class TestSceneEndangerEdButton : EndangerEdTestScene
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        EndangerEdButton button = new EndangerEdButton("Test")
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(220, 100)
        };

        Add(button);
        AddStep("disable button", () => button.Enabled.Value = false);
        AddStep("enable button", () => button.Enabled.Value = true);
    }
}
