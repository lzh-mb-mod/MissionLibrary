using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionLibrary.Category
{
    public abstract class ARepository<TSelf, TItem> : ATag<TSelf> where TSelf : ARepository<TSelf, TItem> where TItem :  AItem<TItem>
    {
        public static TSelf Get()
        {
            return Global.GetInstance<TSelf>();
        }

        public abstract Dictionary<string, IProvider<TItem>> Items { get; }

        public abstract void RegisterItem(IProvider<TItem> category, bool addOnlyWhenMissing = true);
        public abstract TItem GetItem(string categoryId);
        public abstract T GetItem<T>(string categoryId) where T : TItem;
    }
}
