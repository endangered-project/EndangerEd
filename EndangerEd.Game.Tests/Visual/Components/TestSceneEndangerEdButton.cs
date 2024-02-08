using EndangerEd.Game.Graphics;
using osu.Framework.Graphics;
using osuTK;

namespace EndangerEd.Game.Tests.Visual.Components;

public partial class TestSceneEndangerEdButton : EndangerEdTestScene
{
    protected override void LoadComplete()
    {
        base.LoadComplete();

        Add(new EndangerEdButton("Test")
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Size = new Vector2(220, 100)
        });
    }
}
