using MissionLibrary.Event;
using MissionLibrary.View;
using MissionSharedLibrary.HotKey;
using System;

namespace MissionSharedLibrary.View
{
    public class OptionView : MissionMenuViewBase
    {
        public OptionView(int viewOrderPriority, Version version)
            : base(viewOrderPriority, "MissionLibrary" + nameof(OptionView) + "-" + version)
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
            else if (GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed(Input))
                ActivateMenu();
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            MissionLibrary.Event.MissionEvent.Clear();
            AMenuManager.Get().MenuClassCollection.Clear();
        }

        protected override MissionMenuVMBase GetDataSource()
        {
            return new OptionVM(AMenuManager.Get().MenuClassCollection, OnCloseMenu);
        }

        public override void DeactivateMenu()
        {
            if (!IsActivated)
                return;
            base.DeactivateMenu();
            MissionEvent.OnMissionMenuClosed();
        }
    }
}
