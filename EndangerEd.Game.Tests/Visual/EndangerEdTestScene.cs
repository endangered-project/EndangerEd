using EndangerEd.Game.Audio;
using EndangerEd.Game.Screens.ScreenStacks;
using EndangerEd.Game.Stores;
using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace EndangerEd.Game.Tests.Visual
{
    public abstract partial class EndangerEdTestScene : TestScene
    {
        public new DependencyContainer Dependencies { get; set; }

        public EndangerEdMainScreenStack MainScreenStack;

        public SettingsScreenStack SettingsScreenStack;

        public SessionStore SessionStore;
        
        public AudioPlayer AudioPlayer;

        public GameSessionStore GameSessionStore;

        protected override ITestSceneTestRunner CreateRunner() => new EndangerEdTestSceneTestRunner();

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            IReadOnlyDependencyContainer baseDependencies = base.CreateChildDependencies(parent);
            Dependencies = new DependencyContainer(baseDependencies);
            return Dependencies;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Dependencies.CacheAs(this);
            Dependencies.CacheAs(MainScreenStack = new EndangerEdMainScreenStack());
            Dependencies.CacheAs(SessionStore = new SessionStore());
            Dependencies.CacheAs(GameSessionStore = new GameSessionStore());
            Dependencies.CacheAs(SettingsScreenStack = new SettingsScreenStack());
            Dependencies.CacheAs(AudioPlayer = new AudioPlayer("snowmix.mp3"));
            Add(MainScreenStack = new EndangerEdMainScreenStack());
            Add(SettingsScreenStack = new SettingsScreenStack());
        }

        private partial class EndangerEdTestSceneTestRunner : EndangerEdGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
