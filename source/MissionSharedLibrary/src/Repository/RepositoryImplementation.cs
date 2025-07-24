using MissionLibrary.Repository;
using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionSharedLibrary.Category
{
    public class RepositoryImplementation<TItem> : ARepository<RepositoryImplementation<TItem>, TItem> where TItem: AItem<TItem>
    {

        public override Dictionary<string, IProvider<TItem>> Items { get; } = new Dictionary<string, IProvider<TItem>>();

        public override void RegisterItem(IProvider<TItem> provider, bool addOnlyWhenMissing = true)
        {
            if (Items.TryGetValue(provider.Id, out IProvider<TItem> existingProvider))
            {
                if (existingProvider.ProviderVersion == provider.ProviderVersion && addOnlyWhenMissing ||
                    existingProvider.ProviderVersion > provider.ProviderVersion)
                    return;

                Items[provider.Id] = provider;
                return;
            }

            Items.Add(provider.Id, provider);
        }

        public override TItem GetItem(string categoryId)
        {
            if (Items.TryGetValue(categoryId, out IProvider<TItem> provider))
            {
                return provider.Value;
            }

            return null;
        }

        public override T GetItem<T>(string categoryId)
        {
            if (Items.TryGetValue(categoryId, out IProvider<TItem> provider) && provider.Value is T t)
            {
                return t;
            }

            return null;
        }
    }
}
