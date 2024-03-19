using EndangerEd.Game;
using osu.Framework.Platform;

namespace EndangerEd.Desktop;

public partial class EndangerEdGameDesktop : EndangerEdGame
{
    public override void SetHost(GameHost host)
    {
        base.SetHost(host);
        var desktopWindow = host.Window;

        desktopWindow.Title = "EndangerEd";
        base.SetHost(host);
    }
}
