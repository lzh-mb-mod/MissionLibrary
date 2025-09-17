namespace MissionLibrary.Provider
{
    public interface IProviderManager
    {
        void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>;
    }
}
