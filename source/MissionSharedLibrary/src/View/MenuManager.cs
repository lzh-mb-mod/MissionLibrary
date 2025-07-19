using MissionLibrary.View;
using MissionSharedLibrary.View.HotKey;
using MissionSharedLibrary.View.ViewModelCollection;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.View
{
    public class MenuManager : AMenuManager
    {
        public override AMenuClassCollection MenuClassCollection { get; } = new MenuClassCollection();

        // deprecated
        public override MissionView CreateMenuView()
        {
            return new OptionView(24, new Version(1, 2, 0));
        }

        // deprecated
        public override MissionView CreateGameKeyConfigView()
        {
            return new GameKeyConfigView();
        }

        public override void RequestToOpenMenu()
        {
            Mission.Current.GetMissionBehavior<OptionView>()?.ActivateMenu();
        }

        public override void RequestToCloseMenu()
        {
            Mission.Current.GetMissionBehavior<OptionView>()?.DeactivateMenu();
        }

        public override void RequestToOpenUsageView()
        {
            Mission.Current.GetMissionBehavior<UsageView>()?.ActivateMenu();
        }
    }
}
