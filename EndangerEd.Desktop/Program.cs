using osu.Framework.Platform;
using osu.Framework;
using EndangerEd.Game;

namespace EndangerEd.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"EndangerEd"))
            using (osu.Framework.Game game = new EndangerEdGame())
                host.Run(game);
        }
    }
}
