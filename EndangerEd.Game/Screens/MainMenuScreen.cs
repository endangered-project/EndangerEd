using System.Collections.Generic;
using EndangerEd.Game.Audio;
using EndangerEd.Game.Graphics;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace EndangerEd.Game.Screens;

public partial class MainMenuScreen : EndangerEdScreen
{
    private Button startButton;
    private Button leaderboardButton;
    private Container profilePictureContainer;
    private Container guestProfilePicture;
    private Sprite loggedInProfilePicture;
    private Container knowledgeBaseContainer;
    private EndangerEdButton loginButton;
    private Button signUpButton;

    [Resolved]
    private AudioPlayer audioPlayer { get; set; }

    [Resolved]
    private EndangerEdMainScreenStack screenStack { get; set; }

    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private SettingsScreenStack settingsScreenStack { get; set; }

    private void onLoginChanged(ValueChangedEvent<bool> isLoggedIn)
    {
        if (isLoggedIn.NewValue)
        {
            loggedInProfilePicture.FadeIn(500, Easing.OutQuint);
            guestProfilePicture.FadeOut(500, Easing.OutQuint);
            signUpButton.FadeOut(500, Easing.OutQuint);
            loginButton.SetText("Logout");
        }
        else
        {
            loggedInProfilePicture.FadeOut(500, Easing.OutQuint);
            guestProfilePicture.FadeIn(500, Easing.OutQuint);
            signUpButton.FadeIn(500, Easing.OutQuint);
            loginButton.SetText("Login");
        }
    }

    [BackgroundDependencyLoader]
    private void load(TextureStore textureStore)
    {
        Y = 3000f;
        InternalChildren = new Drawable[]
        {
            new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "EndangerEd",
                        Font = EndangerEdFont.GetFont(typeface: EndangerEdFont.Typeface.Comfortaa, size: 100, weight: EndangerEdFont.FontWeight.Bold),
                        Y = -200f
                    },
                    knowledgeBaseContainer = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = -130f,
                        X = 230f,
                        Size = new Vector2(150f),
                        Rotation = 315f,
                        Children = new Drawable[]
                        {
                            new SpriteIcon()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                                Size = new Vector2(0.5f),
                                Icon = FontAwesome.Solid.BookOpen,
                                Colour = Color4.White
                            },
                            new SpriteText()
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Text = "Knowledge base",
                                Font = EndangerEdFont.GetFont(typeface: EndangerEdFont.Typeface.Comfortaa, size: 20, weight: EndangerEdFont.FontWeight.Bold),
                                Y = 40f
                            }
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            startButton = new EndangerEdButton("Start!")
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = -50f,
                                Width = 100,
                                Height = 50,
                                Action = () =>
                                {
                                    screenStack.MainScreenStack.Push(new LoadingScreen());
                                },
                                Enabled = { BindTarget = sessionStore.IsLoggedIn }
                            },
                            leaderboardButton = new EndangerEdButton("Leaderboard")
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 25f,
                                Width = 150,
                                Height = 50
                            },
                            new EndangerEdButton("Settings")
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Y = 100f,
                                Width = 130,
                                Height = 50,
                                Action = () => settingsScreenStack.ToggleVisibility()
                            }
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Y = 200f,
                        Child = new FillFlowContainer
                        {
                            Direction = FillDirection.Horizontal,
                            Children = new List<Drawable>()
                            {
                                {
                                    loginButton = new EndangerEdButton("Login")
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Y = 200f,
                                        Width = 100,
                                        Height = 50,
                                        Margin = new MarginPadding { Right = 120 },
                                        Action = () =>
                                        {
                                            if (sessionStore.IsLoggedIn.Value)
                                                sessionStore.Logout();
                                            else
                                                sessionStore.Login();
                                        }
                                    }
                                },
                                {
                                    profilePictureContainer = new Container()
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Right = 120 },
                                        Children = new Drawable[]
                                        {
                                            loggedInProfilePicture = new Sprite
                                            {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Name = "Profile picture",
                                                Width = 75,
                                                Height = 75,
                                                FillMode = FillMode.Fit,
                                                Texture = textureStore.Get("fuji.png")
                                            },
                                            guestProfilePicture = new Container
                                            {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Width = 75,
                                                Height = 75,
                                                CornerRadius = 100,
                                                Child = new Container()
                                                {
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre,
                                                    RelativeSizeAxes = Axes.Both,
                                                    Masking = true,
                                                    Children = new Drawable[]
                                                    {
                                                        new Circle
                                                        {
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            RelativeSizeAxes = Axes.Both,
                                                            Colour = Colour4.DarkGreen,
                                                            BorderThickness = 10,
                                                            BorderColour = Color4.White,
                                                            Masking = true
                                                        },
                                                        new Container
                                                        {
                                                            Anchor = Anchor.Centre,
                                                            Origin = Anchor.Centre,
                                                            RelativeSizeAxes = Axes.Both,
                                                            Child = new SpriteIcon
                                                            {
                                                                Anchor = Anchor.Centre,
                                                                Origin = Anchor.Centre,
                                                                RelativeSizeAxes = Axes.Both,
                                                                Size = new Vector2(0.5f),
                                                                Icon = FontAwesome.Solid.User,
                                                                Colour = Color4.White
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                {
                                    signUpButton = new EndangerEdButton("Sign up")
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Y = 250f,
                                        Width = 100,
                                        Height = 50,
                                        AlwaysPresent = true
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        sessionStore.IsLoggedIn.BindValueChanged(onLoginChanged, true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        knowledgeBaseContainer.ScaleTo(new Vector2(1.2f), 500, Easing.OutSine).Then().ScaleTo(new Vector2(1f), 500, Easing.OutSine).Loop();
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        base.OnEntering(e);
        this.MoveToY(0f, 1000, Easing.OutQuint)
            .FadeInFromZero(1000, Easing.OutQuint);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        base.OnSuspending(e);
        this.MoveToY(3000f, 1000, Easing.OutQuint)
            .FadeTo(0f, 1000, Easing.OutQuint);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        base.OnResuming(e);
        this.MoveToY(0f, 1000, Easing.OutQuint)
            .FadeInFromZero(1000, Easing.OutQuint);
    }
}
