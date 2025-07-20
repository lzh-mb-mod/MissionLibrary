using MissionLibrary.Controller;
using MissionLibrary.Provider;
using MissionSharedLibrary.Controller;
using MissionSharedLibrary.Controller.Camera;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.View;
using System;
namespace MissionSharedLibrary
{
    // Use ADefaultMissionStartingHandlerAdder as it has no extra methods
    // Only the constructor is used.
    // It's not related with the original purpose of ADefaultMissionStartingHandlerAdder
    public class MissionLibraryVersionManager : ADefaultMissionStartingHandlerAdder
    {
        // Create MissionLibraryVersionManager to call constructor
        public static void RegisterInstances()
        {
            // Create MissionLibraryVersionManager to call constructor
            Global2.GetInstance<ADefaultMissionStartingHandlerAdder>(nameof(MissionLibraryVersionManager));
        }

        public static void RegisterSelf()
        {
            RegisterProvider(() => new MissionLibraryVersionManager(), new Version(1, 0), nameof(MissionLibraryVersionManager));
        }

        public MissionLibraryVersionManager()
        {
            RegisterProviders();
        }

        private void RegisterProviders()
        {
            RegisterProvider(() => new GameKeyCategoryManager(), new Version(2, 0));
            RegisterProvider(() => new CameraControllerManager(), new Version(2, 0));
            RegisterProvider(() => new DefaultMissionStartingHandlerAdder(), new Version(2, 0));
            RegisterProvider(() => new MissionStartingManager(), new Version(2, 0));
            RegisterProvider(() => new MenuManager(), new Version(2, 0));
            RegisterProvider(() => new UsageCategoryManager(), new Version(2, 0));
        }

        private static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
        {
            Global2.RegisterProvider(VersionProviderCreator.Create(creator, providerVersion), key);
        }
    }

}