using osu.Framework.Platform;
using osu.Framework;

namespace EndangerEd.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableDesktopHost(@"EndangerEd"))
            using (osu.Framework.Game game = new EndangerEdGameDesktop())
                host.Run(game);
        }
    }
}
