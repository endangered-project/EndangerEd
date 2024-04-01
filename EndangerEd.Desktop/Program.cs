using osu.Framework.Platform;
using osu.Framework;
using Velopack;

namespace EndangerEd.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            VelopackApp.Build().Run();
            using (GameHost host = Host.GetSuitableDesktopHost(@"EndangerEd"))
            using (osu.Framework.Game game = new EndangerEdGameDesktop())
                host.Run(game);
        }
    }
}
