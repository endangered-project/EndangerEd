using EndangerEd.Game.Screens;
using EndangerEd.Game.Screens.ScreenStacks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace EndangerEd.Game.Stores;

public class SessionStore
{
    public Bindable<bool> IsLoggedIn { get; } = new Bindable<bool>(false);

    public Bindable<bool> IsGameStarted { get; } = new Bindable<bool>(false);

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    [Resolved]
    private EndangerEdMainScreenStack screenStack { get; set; }

    public SessionStore()
    {
        IsLoggedIn.BindValueChanged(isLoggedInChangedEvent =>
        {
            Logger.Log($"🏬 Logged in state changed to {isLoggedInChangedEvent.NewValue}.");
        });
        IsGameStarted.BindValueChanged(isGameStartedChangedEvent =>
        {
            Logger.Log($"🏬 Game started state changed to {isGameStartedChangedEvent.NewValue}.");
        });
    }

    /// <summary>
    /// Login the user.
    /// </summary>
    public void Login()
    {
        if (IsLoggedIn.Value)
        {
            Logger.Log("🏬 User is already logged in.");
        }
        else
        {
            screenStack.Push(new LoginScreen());
        }
    }

    /// <summary>
    /// Logout the user.
    /// </summary>
    public void Logout()
    {
        IsLoggedIn.Value = false;
    }

    /// <summary>
    /// Start the game and also alert the <see cref="GameSessionStore"/> to start the game.
    /// </summary>
    public void StartGame()
    {
        IsGameStarted.Value = true;
    }

    /// <summary>
    /// Stop the game and also alert the <see cref="GameSessionStore"/> to stop the game.
    /// </summary>
    public void StopGame()
    {
        IsGameStarted.Value = false;
    }
}
