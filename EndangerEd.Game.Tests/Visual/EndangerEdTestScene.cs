using osu.Framework.Testing;

namespace EndangerEd.Game.Tests.Visual
{
    public abstract partial class EndangerEdTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new EndangerEdTestSceneTestRunner();

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
