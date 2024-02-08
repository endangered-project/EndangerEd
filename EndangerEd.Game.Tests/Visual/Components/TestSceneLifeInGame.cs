using EndangerEd.Game.Components;
using NUnit.Framework;
using osu.Framework.Allocation;

namespace EndangerEd.Game.Tests.Visual.Components;

[TestFixture]
public partial class TestSceneLifeInGame : EndangerEdTestScene
{
    private LifeInGame lifeInGame;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(lifeInGame = new LifeInGame());
    }

    [Test]
    public void TestLifeInGameNumber()
    {
        AddSliderStep("Life", 0, 20, 3, value => GameSessionStore.Life.Value = value);
        AddStep("remove one heart", () => GameSessionStore.Life.Value -= 1);
        AddRepeatStep("try to remove one heart many times", () => GameSessionStore.Life.Value -= 1, 100);
    }
}
