using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionLibrary
{
    public static class Global
    {
        private static bool IsInitialized { get; set; }
        private static bool IsThirdInitialized { get; set; }

        private static ProviderManager ProviderManager { get; set; }

        public static void Initialize()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;
            ProviderManager = new ProviderManager();
        }

        public static void ThirdInitialize()
        {
            if (IsThirdInitialized)
                return;

            IsThirdInitialized = true;
            ProviderManager.InstantiateAll();
        }

        public static void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>
        {
            ProviderManager.RegisterInstance(newProvider, key);
        }

        public static T GetInstance<T>(string key = "") where T : ATag<T>
        {
            return ProviderManager.GetInstance<T>(key);
        }

        public static IEnumerable<T> GetInstances<T>() where T : ATag<T>
        {
            return ProviderManager.GetInstances<T>();
        }

        public static void Clear()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;
            IsThirdInitialized = false;
            ProviderManager = null;
        }
    }
}
