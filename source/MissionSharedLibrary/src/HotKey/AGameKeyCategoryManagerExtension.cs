using System;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Provider;

namespace MissionSharedLibrary.HotKey
{
    public static class AGameKeyCategoryManagerExtension
    {
        public static void RegisterCategory(this AGameKeyCategoryManager categoryManager, Func<AGameKeyCategory> creator, string id,
            Version version, bool addOnlyWhenMissing = true)
        {
            categoryManager.RegisterItem(new ConcreteProvider<AGameKeyCategory>(creator, id, version), addOnlyWhenMissing);
        }
    }
}
