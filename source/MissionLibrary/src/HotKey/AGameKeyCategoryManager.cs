using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionLibrary.HotKey
{
    public abstract class AGameKeyCategoryManager : ATag<AGameKeyCategoryManager>
    {
        public static AGameKeyCategoryManager Get()
        {
            return Global.GetProvider<AGameKeyCategoryManager>();
        }

        public abstract Dictionary<string, IVersionProvider<AGameKeyCategory>> Categories { get; }
        public abstract void AddCategory(IVersionProvider<AGameKeyCategory> category, bool addOnlyWhenMissing = true);
        public abstract AGameKeyCategory GetCategory(string categoryId);

        public abstract T GetCategory<T>(string categoryId) where T : AGameKeyCategory;
    }
}
