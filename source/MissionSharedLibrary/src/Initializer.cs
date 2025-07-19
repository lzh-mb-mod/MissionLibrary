using MissionLibrary;
using MissionLibrary.Provider;
using MissionLibrary2;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.Utilities;
using System;

namespace MissionSharedLibrary
{
    public class Initializer
    {
        public static bool IsInitialized { get; private set; }
        public static bool IsBeforeSecondInitialized { get; private set; }
        public static bool IsSecondInitialized { get; private set; }

        public static bool Initialize(string moduleId)
        {
            if (IsInitialized)
                return false;

            IsInitialized = true;
            Utility.ModuleId = moduleId;
            Global2.Initialize();
            RegisterProviders();
            return true;
        }

        public static void OnApplicationTick(float dt)
        {
            if (!IsInitialized || IsBeforeSecondInitialized)
                return;

            IsBeforeSecondInitialized = true;
            // This should be called after all calls to Initialize and before all calls to SecondInitialize
            // This is the only point I found.
            RegisterInstancesFromVersionManager();
            return;
        }

        public static bool SecondInitialize()
        {
            if (IsSecondInitialized)
                return false;

            IsSecondInitialized = true;

            Global2.SecondInitialize();
            GeneralGameKeyCategory.RegisterGameKeyCategory();
            return true;
        }

        public static void Clear()
        {
            Global2.Clear();
        }

        private static void RegisterProviders()
        {
            RegisterProvider(() => new MissionLibraryVersionManager(), new Version(1, 0));
        }

        private static void RegisterInstancesFromVersionManager()
        {
            AMissionLibraryVersionManager.Get()?.RegisterInstances();
        }

        public static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
        {
            Global2.RegisterProvider(VersionProviderCreator.Create(creator, providerVersion), key);
        }

    }
}
