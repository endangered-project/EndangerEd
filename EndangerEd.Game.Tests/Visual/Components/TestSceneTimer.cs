using EndangerEd.Game.Graphics;
using EndangerEd.Game.Stores;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace EndangerEd.Game.Tests.Visual.Components;

[TestFixture]
public partial class TestSceneTimer : EndangerEdTestScene
{
    private SpriteText timeText;
    private SpriteText millisecondsText;
    private SpriteText rawTimeText;
    private SpriteText countdownText;

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(timeText = new SpriteText()
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            RelativeSizeAxes = Axes.Both,
            Text = "Time : 0",
            Font = EndangerEdFont.GetFont(size: 20, weight: EndangerEdFont.FontWeight.Bold)
        });
        Add(millisecondsText = new SpriteText()
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Margin = new MarginPadding
            {
                Top = 30
            },
            RelativeSizeAxes = Axes.Both,
            Text = "Milliseconds : 0",
            Font = EndangerEdFont.GetFont(size: 20, weight: EndangerEdFont.FontWeight.Bold)
        });
        Add(rawTimeText = new SpriteText()
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Margin = new MarginPadding
            {
                Top = 60
            },
            RelativeSizeAxes = Axes.Both,
            Text = "Milliseconds : 0",
            Font = EndangerEdFont.GetFont(size: 20, weight: EndangerEdFont.FontWeight.Bold)
        });
        Add(countdownText = new SpriteText()
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft,
            Margin = new MarginPadding
            {
                Top = 90
            },
            RelativeSizeAxes = Axes.Both,
            Text = "Countdown : 0",
            Font = EndangerEdFont.GetFont(size: 20, weight: EndangerEdFont.FontWeight.Bold)
        });
    }

    [Test]
    public void TestScoreDisplayNumber()
    {
        AddStep("start timer", () => GameSessionStore.StopwatchClock.Start());
        AddStep("stop timer", () => GameSessionStore.StopwatchClock.Stop());
        AddStep("reset timer", () => GameSessionStore.StopwatchClock.Reset());
        AddSliderStep<float>("rate", 0f, 1f, 1f, value => GameSessionStore.StopwatchClock.Rate = value);
    }

    protected override void Update()
    {
        base.Update();
        // show time in minutes, seconds
        // also add 0 to seconds if seconds or minutes is less than 10
        timeText.Text = $"Time : {GameSessionStore.StopwatchClock.Elapsed.Minutes}:{(GameSessionStore.StopwatchClock.Elapsed.Seconds < 10 ? "0" : "")}{GameSessionStore.StopwatchClock.Elapsed.Seconds}";
        // show full milliseconds
        millisecondsText.Text = $"Milliseconds : {GameSessionStore.StopwatchClock.Elapsed.Milliseconds}";
        // show raw time
        rawTimeText.Text = $"Raw Time : {GameSessionStore.StopwatchClock}";
        // Calculate countdown
        string countdown = "";
        // Time per game is in milliseconds
        const int total_time = GameSessionStore.TIME_PER_GAME / 1000;
        int timeLeft = total_time - (int)GameSessionStore.StopwatchClock.Elapsed.TotalSeconds;

        // If time left is less than 0, set it to 0
        if (timeLeft < 0)
        {
            timeLeft = 0;
        }

        // Calculate minutes left
        int minutesLeft = timeLeft / 60;
        // Calculate seconds left
        int secondsLeft = timeLeft % 60;
        // Add 0 to seconds if seconds or minutes is less than 10
        countdown += $"{minutesLeft}:{(secondsLeft < 10 ? "0" : "")}{secondsLeft}";
        countdownText.Text = $"Countdown : {countdown}";

        // If time left is less than 10, change color to red
        countdownText.Colour = timeLeft < 10 ? Colour4.Red : Colour4.White;

        countdownText.Text = $"Countdown : {countdown}";
    }
}
