using MissionLibrary.HotKey;
using MissionLibrary.Provider;
using MissionSharedLibrary.Category;
using MissionSharedLibrary.Utilities;
using System;
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
            try
            {
                _repositoryImplementation.RegisterItem(provider, addOnlyWhenMissing);
            }
            catch(Exception e)
            {
                Utility.DisplayMessage(e.ToString());
                Console.WriteLine(e);
            }
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
