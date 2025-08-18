using MissionLibrary.Controller;
using MissionSharedLibrary.Controller.MissionBehaviors;
using MissionSharedLibrary.View;
using MissionSharedLibrary.View.HotKey;
using System;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.Controller
{
    // Deprecated. Use GeneralResourceCreator instead.
    // kept to prevent other mods containing old DefaultMissionStartingHandlerAdder and register things.
    public class DefaultMissionStartingHandlerAdder : ADefaultMissionStartingHandlerAdder
    {
        public DefaultMissionStartingHandlerAdder()
        {
        }
    }

    public class DefaultMissionStartingHandler : AMissionStartingHandler
    {
        public override void OnCreated(MissionView entranceView)
        {
            AddMissionLibraryMissionBehaviors(entranceView);
        }

        public override void OnPreMissionTick(MissionView entranceView, float dt)
        {
        }

        private void AddMissionLibraryMissionBehaviors(MissionView entranceView)
        {
            MissionStartingManager.AddMissionBehavior(entranceView, new MissionLibraryLogic());
            MissionStartingManager.AddMissionBehavior(entranceView, new OptionView(24, new Version(1, 2, 0)));
            MissionStartingManager.AddMissionBehavior(entranceView, new GameKeyConfigView());
            MissionStartingManager.AddMissionBehavior(entranceView, new UsageView(26, new Version(1, 2, 0)));
        }
    }
}
