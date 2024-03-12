using EndangerEd.Game.Graphics;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace EndangerEd.Game.Screens;

public partial class LoginScreen : EndangerEdScreen
{
    [Resolved]
    private SessionStore sessionStore { get; set; }

    [Resolved]
    private EndangerEdConfigManager configManager { get; set; }

    private EndangerEdTextBox usernameTextBox;
    private EndangerEdTextBox passwordTextBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Container()
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = new osuTK.Vector2(0, 10),
                        Children = new Drawable[]
                        {
                            new EndangerEdSpriteText
                            {
                                Text = "Login",
                                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 48f, EndangerEdFont.FontWeight.Bold)
                            },
                            usernameTextBox = new EndangerEdTextBox
                            {
                                Size = new Vector2(220, 50),
                                PlaceholderText = "Username"
                            },
                            passwordTextBox = new EndangerEdTextBox
                            {
                                Size = new Vector2(220, 50),
                                PlaceholderText = "Password"
                            },
                            new EndangerEdButton("Login")
                            {
                                Size = new Vector2(220, 50),
                                Action = login
                            }
                        }
                    }
                }
            },
            new EndangerEdButton("Back")
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Size = new Vector2(100, 50),
                Position = new Vector2(20, -20),
                Action = this.Exit
            }
        };
    }

    private void login()
    {
        // TODO: Do HTTP request to login and store session.
        configManager.SetValue(EndangerEdSetting.AccessToken, "token");
        configManager.SetValue(EndangerEdSetting.RefreshToken, "refresh_token");
        sessionStore.AccessToken = "token";
        sessionStore.IsLoggedIn.Value = true;
        this.Exit();
    }
}
