namespace MissionLibrary.Provider
{
    public interface IProviderManager
    {
        void RegisterProvider<T>(IVersionProvider<T> newProvider) where T : ATag<T>;
    }
}
