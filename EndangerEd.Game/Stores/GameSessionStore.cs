﻿using EndangerEd.Game.API;
using EndangerEd.Game.Screens.ScreenStacks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace EndangerEd.Game.Stores;

public partial class GameSessionStore : CompositeDrawable
{
    [Resolved]
    private EndangerEdMainScreenStack screenStack { get; set; }

    [Resolved]
    private APIRequestManager apiRequestManager { get; set; }

    public const int MAX_LIFE = 3;

    /// <summary>
    /// Time per microgame in milliseconds.
    /// </summary>
    public const int TIME_PER_GAME = 60000;

    public BindableInt Life = new BindableInt(MAX_LIFE);

    public BindableInt Score = new BindableInt();

    public int GameId = 0;

    // We need to use StopwatchClock instead of Stopwatch because it's also depend on the frame time on framework too.
    public StopwatchClock StopwatchClock = new StopwatchClock();

    /// <summary>
    /// Reset the game session to initial state.
    /// </summary>
    public void Reset()
    {
        Life.Value = MAX_LIFE;
        Score.Value = 0;
        StopwatchClock.Reset();
    }

    public bool IsOverTime()
    {
        return StopwatchClock.ElapsedMilliseconds >= TIME_PER_GAME;
    }

    public int GetTimeLeft()
    {
        return (int)((TIME_PER_GAME - StopwatchClock.ElapsedMilliseconds) / 1000);
    }
}
