using MissionSharedLibrary.Usage;
using System;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View
{
    public class UsageVM : MissionMenuVMBase
    {
        public UsageVM(ViewModel usageCollection, Action closeMenu)
            : base(closeMenu)
        {
            UsageCollection = usageCollection;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            UsageCollection.RefreshValues();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            UsageCategoryManager.Get()?.Clear();
        }

        public ViewModel UsageCollection { get; }
    }
}
