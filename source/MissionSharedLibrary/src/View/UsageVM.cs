using MissionLibrary.View;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.View.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

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
