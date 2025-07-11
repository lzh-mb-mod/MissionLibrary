using MissionLibrary.Repository;

namespace MissionLibrary.HotKey
{
    public abstract class AGameKeyCategoryManager : ARepository<AGameKeyCategoryManager, AGameKeyCategory>
    {
        public abstract void Save();
    }
}
