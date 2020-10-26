using MissionSharedLibrary.Config;
using MissionSharedLibrary.src.Config.HotKey;
using System.IO;

namespace MissionLibrary.Config.HotKey
{
    public class MissionLibraryGameKeyConfig : GameKeyConfigBase<MissionLibraryGameKeyConfig>
    {
        protected override string SaveName { get; } = Path.Combine(ConfigPath.ConfigDir, nameof(MissionLibrary), nameof(MissionLibraryGameKeyConfig) + ".xml");
    }
}
