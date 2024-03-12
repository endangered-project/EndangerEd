using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using EndangerEd.Game.API;
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

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

    private EndangerEdTextBox usernameTextBox;
    private EndangerEdTextBox passwordTextBox;
    private TextFlowContainer errorText;

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
                            errorText = new TextFlowContainer
                            {
                                Size = new Vector2(220, 50),
                                Colour = Colour4.Red
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
        Thread thread = new Thread(() =>
        {
            try
            {
                apiRequestManager.PostJson("login", new Dictionary<string, object>
                {
                    { "username", usernameTextBox.Text },
                    { "password", passwordTextBox.Text }
                });
                Scheduler.Add(() =>
                {
                    configManager.SetValue(EndangerEdSetting.AccessToken, "token");
                    configManager.SetValue(EndangerEdSetting.RefreshToken, "refresh_token");
                    sessionStore.AccessToken = "token";
                    sessionStore.IsLoggedIn.Value = true;
                    this.Exit();
                });
            }
            catch (HttpRequestException e)
            {
                Scheduler.Add(() =>
                {
                    // Only show first 100 characters of the error message.
                    string errorMessage = e.Message.Length > 100 ? e.Message.Substring(0, 100) : e.Message;
                    errorText.Text = errorMessage;
                });
            }
        });
        thread.Start();
    }
}
