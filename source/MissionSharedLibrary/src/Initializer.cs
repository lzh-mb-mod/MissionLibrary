using MissionLibrary;
using MissionLibrary.Provider;
using MissionSharedLibrary.Controller;
using MissionSharedLibrary.Controller.Camera;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.Utilities;
using MissionSharedLibrary.View;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using System;

namespace MissionSharedLibrary
{
    public class Initializer
    {
        public static bool IsInitialized { get; private set; }
        public static bool IsSecondInitialized { get; private set; }

        public static bool Initialize(string moduleId)
        {
            if (IsInitialized)
                return false;

            IsInitialized = true;
            Utility.ModuleId = moduleId;
            Global.Initialize();
            RegisterProviders();
            return true;
        }

        public static bool SecondInitialize()
        {
            if (IsSecondInitialized)
                return false;

            IsSecondInitialized = true;
            Global.SecondInitialize();
            GeneralGameKeyCategory.RegisterGameKeyCategory();
            return true;
        }

        public static void Clear()
        {
            Global.Clear();
        }

        private static void RegisterProviders()
        {
            RegisterProvider(() => new GameKeyCategoryManager(), new Version(1, 0));
            RegisterProvider(() => new CameraControllerManager(), new Version(1, 0));
            RegisterProvider(() => new MissionLibraryMissionLogicFactory(), new Version(1, 0));
            RegisterProvider(() => new MissionStartingManager(), new Version(1, 2));
            RegisterProvider(() => new DefaultMissionStartingHandlerAdder(), new Version(1, 1));
            RegisterProvider(() => new MenuManager(), new Version(1, 1));
            RegisterProvider(() => new UsageCategoryManager(), new Version(1, 0));
        }

        public static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
        {
            Global.RegisterProvider(VersionProviderCreator.Create(creator, providerVersion), key);
        }

    }
}
