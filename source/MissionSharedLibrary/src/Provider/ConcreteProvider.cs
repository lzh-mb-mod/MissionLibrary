using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
    public class ConcreteProvider<T> : IProvider<T> where T : ATag<T>
    {
        private readonly Func<ATag<T>> _creator;
        public string Id { get; }

        private T _value;
        public Version ProviderVersion { get; }

        public T Value => _value ??= Create();

        public ConcreteProvider(Func<ATag<T>> creator, string id, Version providerVersion)
        {
            Id = id;
            ProviderVersion = providerVersion;
            _creator = creator;
        }
        public void ForceCreate()
        {
            _value ??= Create();
        }

        public void Clear()
        {
            _value = null;
        }

        private T Create()
        {
            return _creator?.Invoke().Self;
        }
    }

    public class ProviderCreator
    {
        public static ConcreteProvider<T> Create<T>(Func<ATag<T>> creator, string id, Version providerVersion) where T : ATag<T>
        {
            return new ConcreteProvider<T>(creator, id, providerVersion);
        }
    }
}
