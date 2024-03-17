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
    private EndangerEdPasswordBox passwordTextBox;
    private TextFlowContainer errorText;
    private EndangerEdButton loginButton;

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
                                Text = "LOGIN",
                                Font = EndangerEdFont.GetFont(EndangerEdFont.Typeface.JosefinSans, 48f, EndangerEdFont.FontWeight.Bold),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            usernameTextBox = new EndangerEdTextBox
                            {
                                Size = new Vector2(220, 50),
                                PlaceholderText = "Username",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            passwordTextBox = new EndangerEdPasswordBox
                            {
                                Size = new Vector2(220, 50),
                                PlaceholderText = "Password",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            errorText = new TextFlowContainer
                            {
                                Size = new Vector2(220, 50),
                                Colour = Colour4.Red,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            },
                            loginButton = new EndangerEdButton("Login")
                            {
                                Size = new Vector2(220, 50),
                                Action = login,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
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
            Scheduler.Add(() => loginButton.Enabled.Value = false);

            try
            {
                var result = apiRequestManager.PostJson("token", new Dictionary<string, object>
                {
                    { "username", usernameTextBox.Text },
                    { "password", passwordTextBox.Text }
                });
                var accessToken = result.TryGetValue("access", out var token) ? token : null;
                var refreshToken = result.TryGetValue("refresh", out var refresh) ? refresh : null;

                if (accessToken == null || refreshToken == null)
                {
                    throw new HttpRequestException("Invalid response from server");
                }

                Scheduler.Add(() =>
                {
                    configManager.SetValue(EndangerEdSetting.AccessToken, accessToken.ToString());
                    configManager.SetValue(EndangerEdSetting.RefreshToken, refreshToken.ToString());
                    sessionStore.IsLoggedIn.Value = true;
                    this.Exit();
                });
            }
            catch (HttpRequestException e)
            {
                Scheduler.Add(() =>
                {
                    // Only show first 50 characters of the error message.
                    string errorMessage = e.Message.Length > 100 ? e.Message.Substring(0, 50) + "..." : e.Message;
                    errorText.Text = errorMessage;
                    loginButton.Enabled.Value = true;
                });
            }
        });
        thread.Start();
    }
}
