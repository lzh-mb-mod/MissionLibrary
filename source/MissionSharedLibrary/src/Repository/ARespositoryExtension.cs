using MissionLibrary.HotKey;
using MissionLibrary.Repository;
using MissionSharedLibrary.Provider;
using System;

namespace MissionSharedLibrary.Usage
{
    public static class ARespositoryExtension
    {
        public static void RegisterItem<TSelf, TItem>(this ARepository<TSelf, TItem> repository, Func<TItem> creator, string id,
            Version version, bool addOnlyWhenMissing = true) where TSelf : ARepository<TSelf, TItem> where TItem : AItem<TItem>
        {
            repository.RegisterItem(new ConcreteProvider<TItem>(creator, id, version), addOnlyWhenMissing);
        }

        public static void RegisterGameKeyCategory(this AGameKeyCategoryManager manager, Func<AGameKeyCategory> creator, string id, Version version, bool addOnlyWhenMissing = true)
        {
            manager.RegisterItem(new ConcreteProvider<AGameKeyCategory>(() =>
            {
                var result = creator();
                result.Load();
                result.Save();
                return result;
            }, id, version), addOnlyWhenMissing);
        }
    }
}
