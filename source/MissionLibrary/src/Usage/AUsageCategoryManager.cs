using MissionLibrary.HotKey;
using MissionLibrary.Repository;
using MissionLibrary.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
