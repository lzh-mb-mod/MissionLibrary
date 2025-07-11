using System;

namespace MissionLibrary.Provider
{
    public interface IProvider
    {
        string Id { get; }
        Version ProviderVersion { get; }
        void ForceCreate();
        void Clear();
    }

    public interface IProvider<out T> : IProvider where T : ATag<T>
    {
        T Value { get; }
    }
}
