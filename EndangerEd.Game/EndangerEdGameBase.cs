using EndangerEd.Game.API;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;
using EndangerEd.Resources;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics.Performance;

namespace EndangerEd.Game
{
    public partial class EndangerEdGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content { get; }

        private DependencyContainer dependencies;

        private AudioManager audioManager;

        private EndangerEdTextureStore textureStore;

        private Bindable<bool> fpsDisplayVisible;

        private APIEndpointConfig endpointConfig;

        protected EndangerEdConfigManager LocalConfig { get; private set; }

        protected EndangerEdGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768)
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(typeof(EndangerEdResources).Assembly));

            AddFont(Resources, @"Fonts/MPlus1p/MPlus1p-Regular");
            AddFont(Resources, @"Fonts/MPlus1p/MPlus1p-Bold");
            AddFont(Resources, @"Fonts/MPlus1p/MPlus1p-BoldItalic");
            AddFont(Resources, @"Fonts/MPlus1p/MPlus1p-Italic");
            AddFont(Resources, @"Fonts/MPlus1p/MPlus1p-Light");
            AddFont(Resources, @"Fonts/MPlus1p/MPlus1p-Medium");

            AddFont(Resources, @"Fonts/JosefinSans/JosefinSans");
            AddFont(Resources, @"Fonts/JosefinSans/JosefinSans-Bold");
            AddFont(Resources, @"Fonts/JosefinSans/JosefinSans-Italic");
            AddFont(Resources, @"Fonts/JosefinSans/JosefinSans-BoldItalic");

            AddFont(Resources, @"Fonts/YuGothic/YuGothic");
            AddFont(Resources, @"Fonts/YuGothic/YuGothic-Bold");
            AddFont(Resources, @"Fonts/YuGothic/YuGothic-Italic");
            AddFont(Resources, @"Fonts/YuGothic/YuGothic-BoldItalic");

            AddFont(Resources, @"Fonts/Offside/Offside");

            AddFont(Resources, @"Fonts/Comfortaa/Comfortaa-Regular");
            AddFont(Resources, @"Fonts/Comfortaa/Comfortaa-Bold");
            AddFont(Resources, @"Fonts/Comfortaa/Comfortaa-BoldItalic");
            AddFont(Resources, @"Fonts/Comfortaa/Comfortaa-Italic");
            AddFont(Resources, @"Fonts/Comfortaa/Comfortaa-Light");

            AddFont(Resources, @"Fonts/Noto/Noto-Basic");
            AddFont(Resources, @"Fonts/Noto/Noto-Hangul");
            AddFont(Resources, @"Fonts/Noto/Noto-CJK-Basic");
            AddFont(Resources, @"Fonts/Noto/Noto-CJK-Compatibility");
            AddFont(Resources, @"Fonts/Noto/Noto-Thai");

            ResourceStore<byte[]> trackResourceStore = new ResourceStore<byte[]>();
            trackResourceStore.AddStore(new NamespacedResourceStore<byte[]>(Resources, "Tracks"));

            ResourceStore<byte[]> textureResourceStore = new ResourceStore<byte[]>();
            textureResourceStore.AddStore(new NamespacedResourceStore<byte[]>(Resources, "Textures"));
            textureResourceStore.AddStore(new ResourceStore<byte[]>(new EndangerEdStore(Host.Storage)));

            dependencies.Cache(textureStore = new EndangerEdTextureStore(Host.Renderer, Host.CreateTextureLoaderStore(textureResourceStore)));
            dependencies.Cache(audioManager = new AudioManager(Host.AudioThread, trackResourceStore, new NamespacedResourceStore<byte[]>(Resources, "Samples")));
            dependencies.CacheAs(LocalConfig = new EndangerEdConfigManager(Host.Storage));
            dependencies.CacheAs(DebugUtils.IsDebugBuild ? new APIRequestManager(new DevelopmentAPIEndpointConfig()) : new APIRequestManager(new ProductionAPIEndpointConfig()));
            dependencies.CacheAs(endpointConfig = DebugUtils.IsDebugBuild ? new DevelopmentAPIEndpointConfig() : new ProductionAPIEndpointConfig());
            dependencies.CacheAs(this);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            fpsDisplayVisible = LocalConfig.GetBindable<bool>(EndangerEdSetting.ShowFPSCounter);
            fpsDisplayVisible.ValueChanged += visible => { FrameStatistics.Value = visible.NewValue ? FrameStatisticsMode.Full : FrameStatisticsMode.None; };
            fpsDisplayVisible.TriggerChange();
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
