using MissionLibrary.HotKey;
using MissionLibrary.Provider;
using MissionSharedLibrary.Category;
using System.Collections.Generic;

namespace MissionSharedLibrary.HotKey
{
    public class GameKeyCategoryManager : AGameKeyCategoryManager
    {
        private readonly RepositoryImplementation<AGameKeyCategory> _repositoryImplementation = new RepositoryImplementation<AGameKeyCategory>();
        public override Dictionary<string, IProvider<AGameKeyCategory>> Items => _repositoryImplementation.Items;

        public override AGameKeyCategory GetItem(string categoryId)
        {
            return _repositoryImplementation.GetItem(categoryId);
        }

        public override T GetItem<T>(string categoryId)
        {
           return  _repositoryImplementation.GetItem<T>(categoryId);
        }

        public override void RegisterItem(IProvider<AGameKeyCategory> provider, bool addOnlyWhenMissing = true)
        {
            _repositoryImplementation.RegisterItem(provider, addOnlyWhenMissing);

            provider.Value.Load();
            provider.Value.Save();
        }

        public override void Save()
        {
            foreach(var pair in Items)
            {
                pair.Value.Value.Save();
            }
        }
    }
}
