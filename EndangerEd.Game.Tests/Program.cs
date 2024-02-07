using osu.Framework;
using osu.Framework.Platform;

namespace EndangerEd.Game.Tests
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost("visual-tests"))
            using (var game = new EndangerEdTestBrowser())
                host.Run(game);
        }
    }
}
