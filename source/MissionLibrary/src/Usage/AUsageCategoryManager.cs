using MissionLibrary.Repository;
using TaleWorlds.Library;

namespace MissionLibrary.Usage
{
    public abstract class AUsageCategoryManager : ARepository<AUsageCategoryManager, AUsageCategory>
    {
        public abstract void OnUsageCategorySelected(AUsageCategory usageCategory);
        // legacy
        public abstract ViewModel GetViewModel();
        public abstract void Clear();
    }
}
