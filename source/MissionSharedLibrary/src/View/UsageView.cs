using MissionLibrary.Usage;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using System;
using TaleWorlds.Core;

namespace MissionSharedLibrary.View
{
    public class UsageView : MissionMenuViewBase
    {
        public UsageView(int viewOrderPriority, Version version)
            : base(viewOrderPriority, "MissionLibrary" + nameof(UsageView) + "-" + version, false)
        {
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (IsActivated)
            {
                if (GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed())
                    DeactivateMenu();
            }
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            AUsageCategoryManager.Get()?.Clear();
        }

        protected override MissionMenuVMBase GetDataSource()
        {
            return new UsageVM(new UsageCollectionViewModel(GameTexts.FindText("str_mission_library_usages"), AUsageCategoryManager.Get(), OnCloseMenu), OnCloseMenu);
        }
    }
}
