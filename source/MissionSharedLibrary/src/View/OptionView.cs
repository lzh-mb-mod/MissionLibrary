using MissionLibrary.Event;
using MissionLibrary.View;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.View.HotKey;
using System;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View
{
    public class OptionView : MissionMenuViewBase
    {
        public OptionView(int viewOrderPriority, Version version)
            : base(viewOrderPriority, "MissionLibrary" + nameof(OptionView) + "-" + version, !GameNetwork.IsMultiplayer)
        {
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if ((IsActivated || Mission.Mode != TaleWorlds.Core.MissionMode.Conversation) &&
                GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed() &&
                Mission.GetMissionBehavior<GameKeyConfigView>()?.IsActive != true)
            {
                ToggleMenu();
            }
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            MissionEvent.Clear();
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
