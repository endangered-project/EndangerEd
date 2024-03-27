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
        // TODO: Change this to the correct audio track
        audioPlayer.ChangeTrack("snowmix.mp3");
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
                   The game can be segmented into three parts.

                   ## Pre-game
                   When booting up the client, before the player can start playing the game, they will be greeted with the menu.
                   The menu lets the player do the following:
                   - Start playing the video game.
                   - View the scoreboard.
                   - Adjust the client's settings.
                   - Authorization and authentication (login, signup, logout).
                   - Access the knowledge base.
                   ## Gameplay
                   The gameplay involves completing microgames one after another until the game is over. For the condition to be met, the player's
                   lives must be depleted or the game is ended prematurely by the player via pressing the end button. The player is given three lives in total,
                   which acts as a three-strikes system. A life is removed when the player fails a microgame or skip a microgame.
                   The former happens when they choose the wrong answer or the time runs out, while the latter happens when they press the skip button.
                   On the other hand, a player is able to gain points by completing microgames. To keep the gameplay engaging, the game to increases
                   the difficulty the more microgames are completed. It does this by decreasing the time limit for completing each microgame and introducing
                   harder microgames to the game rotation.
                   ## Post-game
                   After the game is over, if the player is logged in, the scores will be sent to the server registration to the scoreboard. The player is able
                   to view their statistics summary and the scoreboard showing their ranking on the post-game interface. They can then choose to either retry
                   or quit to the main menu after they finished viewing the information.
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
                                Icon = FontAwesome.Solid.Anchor,
                                QuestionMode = QuestionMode.Bucket
                            },
                            new MicrogameTutorialContainer()
                            {
                                Name = "Traffic",
                                Description = "Control the traffic lights.",
                                Icon = FontAwesome.Solid.TrafficLight,
                                QuestionMode = QuestionMode.Traffic
                            },
                            new MicrogameTutorialContainer()
                            {
                                Name = "Take Picture",
                                Description = "Take a picture of the jumping fish.",
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
                                Icon = FontAwesome.Solid.Transgender,
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
