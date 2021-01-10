using MissionLibrary.Provider;

namespace MissionLibrary
{
    public static class Global
    {
        private static bool IsInitialized { get; set; }
        private static bool IsSecondInitialized { get; set; }

        private static ProviderManager ProviderManager { get; set; }

        public static void Initialize()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;
            ProviderManager = new ProviderManager();
        }

        public static void SecondInitialize()
        {
            if (IsSecondInitialized)
                return;

            IsSecondInitialized = true;
            ProviderManager.InstantiateAll();
        }

        public static void RegisterProvider<T>(IVersionProvider<T> newProvider) where T : ATag<T>
        {
            ProviderManager.RegisterProvider(newProvider);
        }

        public static T GetProvider<T>() where T : ATag<T>
        {
            return ProviderManager.GetProvider<T>();
        }

        public static void Clear()
        {
            if (!IsInitialized)
                return;

            IsInitialized = false;
            IsSecondInitialized = false;
            ProviderManager = null;
        }
    }
}
