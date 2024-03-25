using EndangerEd.Game.Audio;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace EndangerEd.Game.Screens.Games;

public partial class GameOverScreen : EndangerEdScreen
{
    [Resolved]
    private EndangerEdMainScreenStack mainScreenStack { get; set; }

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private AudioPlayer audioPlayer { get; set; }

    private Container container;
    private Box loadingBox;

    private Sample incorrectAnswerSample;

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManager)
    {
        incorrectAnswerSample = audioManager.Samples.Get("UI/WrongNotify.wav");

        InternalChildren = new Drawable[]
        {
            container = new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(300, 300),
                Masking = true,
                CornerRadius = 20,
                Scale = new Vector2(0),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.75f
                    },
                    new FillFlowContainer()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(500, 500),
                        Direction = FillDirection.Vertical,
                        Masking = true,
                        Spacing = new Vector2(10),
                        Children = new Drawable[]
                        {
                            new SpriteIcon()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Icon = FontAwesome.Solid.TimesCircle,
                                Size = new Vector2(100),
                                Colour = Colour4.Red
                            },
                            new EndangerEdSpriteText()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Game Over".ToUpper(),
                                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 40, EndangerEdFont.FontWeight.Bold),
                                Colour = Colour4.Red
                            }
                        }
                    },
                    loadingBox = new Box()
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        RelativeSizeAxes = Axes.X,
                        Height = 15,
                        Colour = Colour4.White,
                        Alpha = 0.75f
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        audioPlayer.ChangeTrack("ingame.mp3");
        incorrectAnswerSample?.Play();
        container.ScaleTo(1, 1000, Easing.OutElastic);
        loadingBox.ResizeWidthTo(0, 3000);

        Scheduler.AddDelayed(() =>
        {
            mainScreenStack.SwapScreenStack(100);
            mainScreenStack.MainScreenStack.Push(new ResultScreen(gameSessionStore.GameId));
            gameSessionStore.Reset();
        }, 3000);
    }
}
