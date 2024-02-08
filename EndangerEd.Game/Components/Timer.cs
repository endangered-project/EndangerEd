using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace EndangerEd.Game.Components;

public partial class Timer : CompositeDrawable
{
    [Resolved]
    private GameSessionStore gameSessionStore { get; set; }

    private SpriteText timerText;
    private SpriteIcon icon;

    private string lastUpdate = "1:00";

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new SpriteText
            {
                Text = "Time",
                Font = new FontUsage(size: 30),
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding(10),
            },
            new FillFlowContainer
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Direction = FillDirection.Horizontal,
                Margin = new MarginPadding
                {
                    Top = 50,
                    Right = 100,
                },
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    icon = new SpriteIcon
                    {
                        Icon = FontAwesome.Solid.Clock,
                        Size = new Vector2(30),
                    },
                    timerText = new SpriteText
                    {
                        Text = gameSessionStore.Score.Value.ToString(),
                        Font = new FontUsage(size:30),
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        // Calculate countdown
        string countdown = "";
        // Time per game is in milliseconds
        const int total_time = GameSessionStore.TIME_PER_GAME / 1000;
        int timeLeft = total_time - (int)gameSessionStore.StopwatchClock.Elapsed.TotalSeconds;

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
        timerText.Text = $"{countdown}";

        // If the countdown has changed, update the lastUpdate variable
        if (countdown != lastUpdate)
        {
            lastUpdate = countdown;
            icon.FlashColour(Colour4.Red, 500, Easing.OutQuint);
        }
    }
}
