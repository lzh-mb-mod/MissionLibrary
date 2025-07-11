using MissionLibrary.HotKey;
using MissionLibrary.Provider;
using MissionSharedLibrary.Category;
using System.Collections.Generic;

namespace MissionSharedLibrary.HotKey
{
    public class GameKeyCategoryManager : AGameKeyCategoryManager
    {
        private readonly CategoryManager<AGameKeyCategory> _categoryManagerImplementation = new CategoryManager<AGameKeyCategory>();
        public override Dictionary<string, IVersionProvider<AGameKeyCategory>> Categories => _categoryManagerImplementation.Categories;

        public override AGameKeyCategory GetCategory(string categoryId)
        {
            return _categoryManagerImplementation.GetCategory(categoryId);
        }

        public override T GetCategory<T>(string categoryId)
        {
           return  _categoryManagerImplementation.GetCategory<T>(categoryId);
        }

        public override void RegisterCategory(IVersionProvider<AGameKeyCategory> provider, bool addOnlyWhenMissing = true)
        {
            _categoryManagerImplementation.RegisterCategory(provider, addOnlyWhenMissing);

            provider.Value.Load();
            provider.Value.Save();
        }

        public override void Save()
        {
            foreach(var pair in Categories)
            {
                pair.Value.Value.Save();
            }
        }
    }
}
