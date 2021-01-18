using System;
using System.Collections.Generic;

namespace MissionLibrary.Provider
{
    public class ProviderManager : IProviderManager
    {
        private readonly Dictionary<Type, IVersionProvider> _providers = new Dictionary<Type, IVersionProvider>();

        public void RegisterProvider<T>(IVersionProvider<T> newProvider) where T : ATag<T>
        {
            if (!_providers.TryGetValue(typeof(T), out IVersionProvider oldProvider))
            {
                _providers.Add(typeof(T), newProvider);
            }
            else if (oldProvider.ProviderVersion.CompareTo(newProvider.ProviderVersion) <= 0)
            {
                _providers[typeof(T)] = newProvider;
            }
        }

        public T GetProvider<T>() where T : ATag<T>
        {
            if (!_providers.TryGetValue(typeof(T), out IVersionProvider provider) || !(provider is IVersionProvider<T> tProvider))
            {
                return null;
            }

            return tProvider.Value;
        }

        public void InstantiateAll()
        {
            foreach (var pair in _providers)
            {
                pair.Value.ForceCreate();
            }
        }
    }
}
