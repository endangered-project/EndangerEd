using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace EndangerEd.Game.Stores;

public class EndangerEdConfigManager : IniConfigManager<EndangerEdSetting>
{
    public EndangerEdConfigManager(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        SetDefault(EndangerEdSetting.ShowFPSCounter, false);
    }
}
