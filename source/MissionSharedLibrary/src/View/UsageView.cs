using MissionLibrary.Event;
using MissionLibrary.View;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Usage;
using System;

namespace MissionSharedLibrary.View
{
    public class UsageView : MissionMenuViewBase
    {
        public UsageView(int viewOrderPriority, Version version)
            : base(viewOrderPriority, "MissionLibrary" + nameof(UsageView) + "-" + version)
        {
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (IsActivated)
            {
                if (GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed(GauntletLayer.Input))
                    DeactivateMenu();
            }
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            UsageCategoryManager.Get()?.Clear();
        }

        protected override MissionMenuVMBase GetDataSource()
        {
            return new UsageVM(UsageCategoryManager.Get().GetViewModel(), OnCloseMenu);
        }
    }
}
