using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionLibrary.Category
{
    public abstract class ACategoryManager<TSelf, TCategory> : ATag<TSelf> where TSelf : ACategoryManager<TSelf, TCategory> where TCategory :  ACategory<TCategory>
    {
        public static TSelf Get()
        {
            return Global.GetInstance<TSelf>();
        }

        public abstract Dictionary<string, IVersionProvider<TCategory>> Categories { get; }

        public abstract void RegisterCategory(IVersionProvider<TCategory> category, bool addOnlyWhenMissing = true);
        public abstract TCategory GetCategory(string categoryId);
        public abstract T GetCategory<T>(string categoryId) where T : TCategory;

    }
}
