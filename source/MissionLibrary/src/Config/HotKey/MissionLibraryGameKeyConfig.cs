using MissionSharedLibrary.Config;
using System.IO;
using MissionSharedLibrary.Config.HotKey;

namespace MissionLibrary.Config.HotKey
{
    public class MissionLibraryGameKeyConfig : GameKeyConfigBase<MissionLibraryGameKeyConfig>
    {
        protected override string SaveName { get; } = Path.Combine(ConfigPath.ConfigDir, nameof(MissionLibrary), nameof(MissionLibraryGameKeyConfig) + ".xml");
    }
}
