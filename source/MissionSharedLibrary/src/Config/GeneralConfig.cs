using MissionSharedLibrary.Utilities;
using System;
using System.IO;
using System.Xml.Serialization;

namespace MissionSharedLibrary.Config
{
    public class GeneralConfig : MissionConfigBase<GeneralConfig>
    {
        protected static Version BinaryVersion => new Version(1, 0);
        public string ConfigVersion { get; set; } = BinaryVersion.ToString();

        public string PreviouslySelectedOptionClassId = "RTSCamera";

        public static void OnMenuClosed()
        {
            Get().Serialize();
        }

        protected override void CopyFrom(GeneralConfig other)
        {
            ConfigVersion = other.ConfigVersion;
            PreviouslySelectedOptionClassId = other.PreviouslySelectedOptionClassId;
        }

        protected override void UpgradeToCurrentVersion()
        {
            switch (ConfigVersion)
            {
                default:
                    Utility.DisplayLocalizedText("str_rts_camera_config_incompatible");
                    ResetToDefault();
                    Serialize();
                    goto case "1.0";
                case "1.0":
                    break;
            }

            ConfigVersion = BinaryVersion.ToString(2);
        }

        [XmlIgnore]
        protected override string SaveName => Path.Combine(ConfigPath.ConfigDir, "RTSCamera", nameof(GeneralConfig) + ".xml");
    }
}