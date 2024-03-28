using EndangerEd.Game.Audio;
using EndangerEd.Game.Components;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Objects;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK;
using Box = osu.Framework.Graphics.Shapes.Box;

namespace EndangerEd.Game.Screens;

public partial class TutorialScreen : EndangerEdScreen
{
    private SpriteText menuTitleSpriteText;

    private Container leftMenuContainer;
    private Container rightMenuContainer;
    private BasicScrollContainer mainContentScrollContainer;

    private EndangerEdButton introductionButton;
    private EndangerEdButton microgameButton;

    private readonly Bindable<TutorialMenu> tutorialMenu = new Bindable<TutorialMenu>();

    [Resolved]
    private AudioPlayer audioPlayer { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            menuTitleSpriteText = new EndangerEdSpriteText()
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Position = new Vector2(-600, 20),
                Text = "Tutorial".ToUpper(),
                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 48f, EndangerEdFont.FontWeight.Bold),
            },
            leftMenuContainer = new Container()
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.25f, 0.75f),
                Masking = true,
                CornerRadius = 16,
                Margin = new MarginPadding()
                {
                    Left = 20
                },
                Position = new Vector2(-600, 0),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4Extensions.FromHex("5F5F66"),
                        Alpha = 0.8f
                    },
                    new BasicScrollContainer()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Child = new FillFlowContainer()
                        {
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 16),
                            Padding = new MarginPadding(15),
                            Children = new Drawable[]
                            {
                                introductionButton = new EndangerEdButton("Introduction")
                                {
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    RelativeSizeAxes = Axes.X,
                                    Size = new Vector2(1, 50),
                                    Action = loadTutorialMenu
                                },
                                microgameButton = new EndangerEdButton("Microgame")
                                {
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    RelativeSizeAxes = Axes.X,
                                    Size = new Vector2(1, 50),
                                    Action = loadMicrogameMenu
                                }
                            }
                        }
                    }
                }
            },
            rightMenuContainer = new Container()
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.7f, 0.75f),
                Masking = true,
                CornerRadius = 16,
                Margin = new MarginPadding()
                {
                    Right = 20
                },
                Position = new Vector2(600, 0),
                Children = new Drawable[]
                {
                    new Box()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4Extensions.FromHex("5F5F66"),
                        Alpha = 0.8f
                    },
                    mainContentScrollContainer = new BasicScrollContainer()
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Name = "Main Content"
                    }
                }
            }
        };

        AddInternal(new EndangerEdButton("Back")
        {
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Size = new Vector2(100, 50),
            Position = new Vector2(20, -20),
            Action = this.Exit
        });

        tutorialMenu.Value = TutorialMenu.Introduction;
        loadTutorialMenu();
        tutorialMenu.BindValueChanged(e =>
        {
            switch (e.OldValue)
            {
                case TutorialMenu.Introduction:
                    introductionButton.SetColour(Colour4.Green);
                    break;

                case TutorialMenu.Microgames:
                    microgameButton.SetColour(Colour4.Green);
                    break;
            }

            switch (e.NewValue)
            {
                case TutorialMenu.Introduction:
                    introductionButton.SetColour(Colour4.DarkSeaGreen);
                    break;

                case TutorialMenu.Microgames:
                    microgameButton.SetColour(Colour4.DarkSeaGreen);
                    break;
            }
        }, true);
    }

    protected override void LoadComplete()
    {
        audioPlayer.ChangeTrack("tutorial.wav");
        base.LoadComplete();
    }

    private void loadTutorialMenu()
    {
        tutorialMenu.Value = TutorialMenu.Introduction;
        mainContentScrollContainer.Child = new MarkdownContainer()
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Margin = new MarginPadding()
            {
                Top = 20,
                Bottom = 20
            },
            Text = """
                   # Welcome to EndangerEd!
                   EndangerEd is about using knowledge to endure through a continuous and escalating dexterity test.
                   Since its questions are assembled from the knowledge base, we encourage you to first read through it before jumping in. 
                   There are several mechanics worth noting when tackling this game.

                   ## Lives
                   You are given 3 lives. A life is deducted when:
                   - You failed to complete a microgame
                   - You skip a microgame
                   - You ran out of time for a microgame

                   Lives cannot be regained, use them wisely!
                   
                   ## Score
                   Each question is worth 50 points. Your score is sent to the leaderboard only if you fully completed a game.

                   ## Time
                   Time starts big but shrinks everytime a microgame concludes. The minimum time the game can go down to is 10 seconds.

                   ## Question Weighing
                   Feel like getting too good at a topic? Don't worry, the game will mix up the pace by including questions outside your
                   comfort zone to keep you on your toes.
                   """
        };
    }

    private void loadMicrogameMenu()
    {
        tutorialMenu.Value = TutorialMenu.Microgames;
        mainContentScrollContainer.Children = new Drawable[]
        {
            new FillFlowContainer()
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Width = 1f,
                Margin = new MarginPadding(20),
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    new FillFlowContainer()
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Width = 1f,
                        Children = new Drawable[]
                        {
                            new MicrogameTutorialContainer()
                            {
                                Name = "Bucket",
                                Description = "Catch the falling objects with the bucket.",

                                Icon = FontAwesome.Solid.ChevronDown,
                                QuestionMode = QuestionMode.Bucket
                            },
                            new MicrogameTutorialContainer()
                            {
                                Name = "Traffic",
                                Description = "Let the correct car through the traffic.",
                                Icon = FontAwesome.Solid.TrafficLight,
                                QuestionMode = QuestionMode.Traffic
                            },
                            new MicrogameTutorialContainer()
                            {
                                Name = "Photograph",
                                Description = "Take a picture of the correct target.",
                                Icon = FontAwesome.Solid.Camera,
                                QuestionMode = QuestionMode.TakePicture
                            }
                        }
                    },
                    new FillFlowContainer()
                    {
                        Anchor = Anchor.TopLeft,
                        Origin = Anchor.TopLeft,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Width = 1f,
                        Children = new Drawable[]
                        {
                            new MicrogameTutorialContainer()
                            {
                                Name = "Conveyor",
                                Description = "Remove the defective products from the conveyor belt before they reach the end.",
                                Icon = FontAwesome.Solid.Cog,
                                QuestionMode = QuestionMode.Conveyor
                            },
                            new MicrogameTutorialContainer()
                            {
                                Name = "Cannon",
                                Description = "Fire the cannon to hit the target.",
                                Icon = FontAwesome.Solid.Bullseye,
                                QuestionMode = QuestionMode.Cannon
                            }
                        }
                    }
                }
            }
        };
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        audioPlayer.ChangeTrack("menu.mp3");
        menuTitleSpriteText.MoveToX(20, 500, Easing.OutQuart);
        leftMenuContainer.MoveToX(0, 500, Easing.OutQuart);
        rightMenuContainer.MoveToX(0, 500, Easing.OutQuart);
        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        audioPlayer.ChangeTrack("menu.mp3");
        menuTitleSpriteText.MoveToX(-600, 500, Easing.OutQuart);
        leftMenuContainer.MoveToX(-600, 500, Easing.OutQuart);
        rightMenuContainer.MoveToX(600, 500, Easing.OutQuart);
        return base.OnExiting(e);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        menuTitleSpriteText.MoveToX(-600, 500, Easing.OutQuart);
        leftMenuContainer.MoveToX(-600, 600, Easing.OutQuart);
        rightMenuContainer.MoveToX(600, 750, Easing.OutQuart);
        base.OnSuspending(e);
    }

    private enum TutorialMenu
    {
        Introduction,
        Microgames
    }
}
