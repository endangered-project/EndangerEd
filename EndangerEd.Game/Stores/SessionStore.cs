using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace EndangerEd.Game.Stores;

public partial class SessionStore : CompositeDrawable
{
    public Bindable<bool> IsLoggedIn { get; } = new Bindable<bool>(false);

    public Bindable<bool> IsGameStarted { get; } = new Bindable<bool>(false);

    public BindableBool IsLoading { get; } = new BindableBool(false);

    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

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
