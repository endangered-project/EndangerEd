using System;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace EndangerEd.Game.Audio;

/// <summary>
/// A global audio player using for play track and to make sure that there is only one track playing at a time.
/// </summary>
public partial class AudioPlayer : CompositeDrawable
{
    public Track Track;
    public Bindable<string> TrackName;
    private ITrackStore trackStore;
    private readonly bool startOnLoaded;

    [Resolved]
    private AudioManager audioManager { get; set; }

    public AudioPlayer(string trackName, bool startOnLoaded = false)
    {
        this.startOnLoaded = startOnLoaded;
        TrackName = new Bindable<string>(trackName);
    }

    [BackgroundDependencyLoader]
    private void load(AudioManager audioManagerSource)
    {
        trackStore = audioManagerSource.Tracks;

        if (TrackName != null)
        {
            Track = trackStore.Get(TrackName.Value);
            TrackName = new Bindable<string>(TrackName.Value);

            if (startOnLoaded)
            {
                Track.StartAsync().WaitSafely();
            }

            Logger.Log("🎵 Initialize AudioPlayer with track " + TrackName.Value);
        }
        else
        {
            TrackName = new Bindable<string>();

            Logger.Log("🎵 Initialize AudioPlayer with no track");
        }

        if (Track != null)
        {
            Track.Looping = true;
        }
    }

    /// <summary>
    /// Play or resumed the <see cref="Track"/>.
    /// </summary>
    public void Play()
    {
        if (Track == null) return;

        if (Track.IsRunning) return;

        Track.StartAsync().WaitSafely();
        Logger.Log("🎵 Resumed track");
    }

    /// <summary>
    /// Pause the <see cref="Track"/>.
    /// </summary>
    public void Pause()
    {
        if (Track == null) return;

        if (!Track.IsRunning) return;

        Track.StopAsync().WaitSafely();
        Logger.Log("🎵 Paused track");
    }

    /// <summary>
    /// Toggle the <see cref="Track"/> playing. If the track is playing, it will be paused. If the track is paused, it will be resumed.
    /// Using <see cref="Play"/> and <see cref="Pause"/> instead of this method if you want to make sure that the track is playing or paused.
    /// </summary>
    public void TogglePlay()
    {
        if (Track == null) return;

        if (Track.IsRunning)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }

    /// <summary>
    /// Toggle the <see cref="Track"/> looping.
    /// </summary>
    public void Loop()
    {
        if (Track == null) return;

        Track.Looping = !Track.Looping;
        Logger.Log("🎵 Toggled track looping to " + Track.Looping);
    }

    /// <summary>
    /// Change the <see cref="Track"/> to the specified track.
    /// </summary>
    /// <param name="trackName">A track name to change</param>
    /// <param name="playAfterChange">Schedule to play after successfully change the track.</param>
    public void ChangeTrack(string trackName, bool playAfterChange = true, bool loop = true)
    {
        try
        {
            // Don't change track if the track is already playing
            if (TrackName.Value == trackName)
            {
                return;
            }

            Scheduler.Add(() =>
            {
                // Stop the current track
                Track?.StopAsync().WaitSafely();
                Track = trackStore.Get(trackName);

                if (loop && Track != null)
                {
                    Track.Looping = true;
                }

                TrackName.Value = trackName;
                Logger.Log("🎵 Changed track to " + trackName);

                if (playAfterChange && Track != null)
                {
                    Track.StartAsync().WaitSafely();
                }
            });
        }
        catch (Exception e)
        {
            Logger.Log("🎵 Failed to change track to " + trackName + " with error: " + e.Message);
        }
    }
}
