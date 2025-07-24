using MissionSharedLibrary.Utilities;
using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Config
{
    public class GeneralConfig : MissionConfigBase<GeneralConfig>
    {
        protected static Version BinaryVersion => new Version(1, 0);
        public string ConfigVersion { get; set; } = BinaryVersion.ToString();

        public string PreviouslySelectedOptionClassId = "RTSCamera";
        public bool HasUsageShown { get; set; }

        public static void OnMenuClosed()
        {
            Get().Serialize();
        }

        protected override void CopyFrom(GeneralConfig other)
        {
            ConfigVersion = other.ConfigVersion;
            PreviouslySelectedOptionClassId = other.PreviouslySelectedOptionClassId;
            HasUsageShown = other.HasUsageShown;
        }

        protected override void UpgradeToCurrentVersion()
        {
            switch (ConfigVersion)
            {
                default:
                    Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_config_incompatible").ToString(), new TaleWorlds.Library.Color(1, 0, 0));
                    ResetToDefault();
                    Serialize();
                    goto case "1.0";
                case "1.0":
                    break;
            }

            ConfigVersion = BinaryVersion.ToString(2);
        }

        [XmlIgnore]
        protected override string SaveName => Path.Combine(ConfigPath.ConfigDir, nameof(MissionLibrary), nameof(GeneralConfig) + ".xml");
    }
}