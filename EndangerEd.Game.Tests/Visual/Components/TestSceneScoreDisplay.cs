using EndangerEd.Game.Components;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace EndangerEd.Game.Tests.Visual.Components;

[TestFixture]
public partial class TestSceneScoreDisplay : EndangerEdTestScene
{
    private ScoreDisplay scoreDisplay;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(scoreDisplay = new ScoreDisplay()
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            RelativeSizeAxes = Axes.Both,
        });
    }

    [Test]
    public void TestScoreDisplayNumber()
    {
        AddSliderStep("Score", 0, 100000, 100, value => GameSessionStore.Score.Value = value);
    }
}
