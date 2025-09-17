using MissionLibrary.Provider;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.Utilities;
using System;

namespace MissionSharedLibrary
{
    public class Initializer
    {
        public static bool IsInitialized { get; private set; }
        public static bool IsInstancesRegisteredFromVersionManager { get; private set; }
        public static bool IsSecondInitialized { get; private set; }

        public static bool IsThirdInitialized { get; private set; }

        public static bool Initialize(string moduleId)
        {
            if (IsInitialized)
                return false;

            IsInitialized = true;
            Utility.ModuleId = moduleId;
            Global2.Initialize();
            RegisterVersionManager();
            return true;
        }

        public static void OnApplicationTick(float dt)
        {
            if (!IsInitialized || IsSecondInitialized)
                return;

            SecondInitialize();
            return;
        }

        private static bool SecondInitialize()
        {
            if (IsSecondInitialized)
                return false;

            IsSecondInitialized = true;

            RegisterInstancesFromVersionManager();
            return true;
        }

        public static bool ThirdInitialize()
        {
            if (IsThirdInitialized)
            {
                return false;
            }

            IsThirdInitialized = true;
            Global2.ThirdInitialize();
            return true;
        }

        public static void Clear()
        {
            Global2.Clear();
        }

        private static void RegisterVersionManager()
        {
            MissionLibraryVersionManager.RegisterSelf();
        }

        private static void RegisterInstancesFromVersionManager()
        {
            MissionLibraryVersionManager.RegisterInstances();
        }

        public static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
        {
            Global2.RegisterInstance(VersionProviderCreator.Create(creator, providerVersion), key);
        }

    }
}
