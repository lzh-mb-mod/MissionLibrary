using MissionLibrary.Controller;
using MissionSharedLibrary.Controller;
using MissionSharedLibrary.View.HotKey;
using System;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.View
{

    public class MenuViewAdder : AMissionStartingHandler
    {
        public override void OnCreated(MissionView entranceView)
        {
            AddMenuViews(entranceView);
        }

        public override void OnPreMissionTick(MissionView entranceView, float dt)
        {
        }

        private void AddMenuViews(MissionView entranceView)
        {
            MissionStartingManager.AddMissionBehavior(entranceView, new OptionView(24, new Version(1, 4, 0)));
            MissionStartingManager.AddMissionBehavior(entranceView, new GameKeyConfigView(new Version(3, 0, 0)));
            MissionStartingManager.AddMissionBehavior(entranceView, new UsageView(26, new Version(1, 2, 0)));
        }
    }
}
