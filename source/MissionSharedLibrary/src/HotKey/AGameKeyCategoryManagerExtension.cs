using System;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Provider;

namespace MissionSharedLibrary.HotKey
{
    public static class AGameKeyCategoryManagerExtension
    {
        public static void RegisterCategory(this AGameKeyCategoryManager categoryManager, Func<AGameKeyCategory> creator,
            Version version, bool addOnlyWhenMissing = true)
        {
            categoryManager.RegisterCategory(new ConcreteVersionProvider<AGameKeyCategory>(creator, version), addOnlyWhenMissing);
        }
    }
}
