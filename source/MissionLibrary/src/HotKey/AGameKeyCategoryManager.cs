using System.Collections.Generic;
using MissionLibrary.Category;
using MissionLibrary.Provider;

namespace MissionLibrary.HotKey
{
    public abstract class AGameKeyCategoryManager : ACategoryManager<AGameKeyCategoryManager, AGameKeyCategory>
    {
        public abstract void Save();
    }
}
