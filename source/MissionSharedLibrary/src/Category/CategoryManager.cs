using MissionLibrary.Category;
using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionSharedLibrary.Category
{
    public class CategoryManager<TCategory> : ACategoryManager<CategoryManager<TCategory>, TCategory> where TCategory: ACategory<TCategory>
    {

        public override Dictionary<string, IVersionProvider<TCategory>> Categories { get; } = new Dictionary<string, IVersionProvider<TCategory>>();

        public override void RegisterCategory(IVersionProvider<TCategory> provider, bool addOnlyWhenMissing = true)
        {
            if (Categories.TryGetValue(provider.Value.CategoryId, out IVersionProvider<TCategory> existingProvider))
            {
                if (existingProvider.ProviderVersion == provider.ProviderVersion && addOnlyWhenMissing ||
                    existingProvider.ProviderVersion > provider.ProviderVersion)
                    return;

                Categories[provider.Value.CategoryId] = provider;
            }

            Categories.Add(provider.Value.CategoryId, provider);
        }

        public override TCategory GetCategory(string categoryId)
        {
            if (Categories.TryGetValue(categoryId, out IVersionProvider<TCategory> provider))
            {
                return provider.Value;
            }

            return null;
        }

        public override T GetCategory<T>(string categoryId)
        {
            if (Categories.TryGetValue(categoryId, out IVersionProvider<TCategory> provider) && provider.Value is T t)
            {
                return t;
            }

            return null;
        }
    }
}
