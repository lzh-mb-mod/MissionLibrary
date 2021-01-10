using MissionLibrary;
using MissionLibrary.Provider;
using MissionSharedLibrary.Controller;
using MissionSharedLibrary.Controller.Camera;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.HotKey.Category;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.View;
using System;

namespace MissionSharedLibrary
{
    public class Initializer
    {
        public static bool IsInitialized { get; private set; }
        public static bool IsSecondInitialized { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;
            Global.Initialize();
            RegisterProviders();
        }

        public static void SecondInitialize()
        {
            if (IsSecondInitialized)
                return;

            IsSecondInitialized = true;
            Global.SecondInitialize();
            GeneralGameKeyCategories.RegisterGameKeyCategory();
        }

        public static void Clear()
        {
            Global.Clear();
        }

        private static void RegisterProviders()
        {
            RegisterProvider(() => new GameKeyCategoryManager(), new Version(1, 0));
            RegisterProvider(() => new CameraControllerManager(), new Version(1, 0));
            RegisterProvider(() => new InputControllerFactory(), new Version(1, 0));
            RegisterProvider(() => new MissionStartingManager(), new Version(1, 0));
            RegisterProvider(() => new DefaultMissionStartingHandlerAdder(), new Version(1, 0));
            RegisterProvider(() => new MenuManager(), new Version(1, 0));
        }

        public static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion) where T : ATag<T>
        {
            Global.RegisterProvider(VersionProviderCreator.Create(creator, providerVersion));
        }

    }
}
