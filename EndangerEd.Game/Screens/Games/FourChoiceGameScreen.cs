using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace EndangerEd.Game.Screens.Games;

public partial class FourChoiceGameScreen(Question question) : MicroGameScreen(question)
{
    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new EndangerEdSpriteText()
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Text = CurrentQuestion.QuestionText,
                Font = EndangerEdFont.GetFont(size: 40)
            },
            new FillFlowContainer()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new osuTK.Vector2(0, 10),
                Children = new Drawable[]
                {
                    new EndangerEdButton(CurrentQuestion.Choices[0])
                    {
                        Size = new osuTK.Vector2(200, 50)
                    },
                    new EndangerEdButton(CurrentQuestion.Choices[1])
                    {
                        Size = new osuTK.Vector2(200, 50)
                    },
                    new EndangerEdButton(CurrentQuestion.Choices[2])
                    {
                        Size = new osuTK.Vector2(200, 50)
                    },
                    new EndangerEdButton(CurrentQuestion.Choices[3])
                    {
                        Size = new osuTK.Vector2(200, 50)
                    }
                }
            }
        };
    }
}
