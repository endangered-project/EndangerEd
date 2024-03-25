using System.IO;
using System.Threading;
using EndangerEd.Game.Components;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace EndangerEd.Game.Graphics;

public partial class OnlineImageButton : Button
{
    private readonly string url;

    private AllowHttpOnlineStore onlineStore = new AllowHttpOnlineStore();

    private Sprite imageSprite;

    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private EndangerEdTextureStore textureStore { get; set; }

    private Texture fallbackTexture;

    public OnlineImageButton(string url)
    {
        this.url = url;
    }

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
                Masking = true,
                CornerRadius = 20,
                BorderColour = Colour4.White,
                BorderThickness = 5,
                Children = new Drawable[]
                {
                    imageSprite = new Sprite
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both
                    }
                },
            },
            new ClickHoverSounds(Enabled)
        };

        fallbackTexture = textureStore.Get("fallback-choice.jpg");
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Thread getImageThread = new Thread(() =>
        {
            Stream stream = onlineStore.GetStream(url);

            try
            {
                imageSprite.Texture = Texture.FromStream(host.Renderer, stream);
                Logger.Log($"Loaded image from {url}", LoggingTarget.Runtime, LogLevel.Debug);
            }
            catch (System.Exception e)
            {
                Logger.Log($"Failed to load image from {url}: {e.Message}, using fallback image", LoggingTarget.Runtime, LogLevel.Error);
                imageSprite.Texture = fallbackTexture;
            }

            stream?.Dispose();
        });
        getImageThread.Start();
    }

    protected override bool OnHover(HoverEvent e)
    {
        this.FlashColour(Colour4.White, 100);
        return base.OnHover(e);
    }
}
