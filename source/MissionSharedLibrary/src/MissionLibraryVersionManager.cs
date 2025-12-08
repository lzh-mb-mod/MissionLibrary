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
            // Multiple mods may register MissionLibraryVersionManager, but
            // only the one with the highest version will be selected.
            // MissionLibraryVersionManager with lower version registered by old mods will be ignored.
            RegisterInstance(() => new MissionLibraryVersionManager(), new Version(1, 6), nameof(MissionLibraryVersionManager));
        }

        public MissionLibraryVersionManager()
        {
            RegisterProviders();
        }

        private void RegisterProviders()
        {
            // old mods that don't use MissionLibraryVersionManager may register the followings with version 1.x
            // and MissionLibraryVersionManager will overwrite them because the versions are 2.x above.
            // If we increment any of below versions, we should also increment version of MissionLibraryVersionManager.
            // so that the mod with the highest version of MissionLibraryVersionManager will register all the below instances.
            RegisterInstance(() => new GameKeyCategoryManager(), new Version(2, 0));
            RegisterInstance(() => new CameraControllerManager(), new Version(2, 0));
            RegisterInstance(() => new DefaultMissionStartingHandlerAdder(), new Version(2, 0));
            RegisterInstance(() => new MissionStartingManager(), new Version(2, 0));
            RegisterInstance(() => new MenuManager(), new Version(2, 0));
            RegisterInstance(() => new UsageCategoryManager(), new Version(2, 0));
            RegisterInstance(() => new GeneralResourceCreator(), new Version(2, 0));
        }

        private static void RegisterInstance<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
        {
            Global2.RegisterInstance(VersionProviderCreator.Create(creator, providerVersion), key);
        }
    }

}