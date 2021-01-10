using System;
using System.Collections.Generic;
using System.Text;
using MissionLibrary;
using MissionLibrary.Controller;
using MissionLibrary.View;
using MissionSharedLibrary.View.HotKey;
using TaleWorlds.MountAndBlade.View.Missions;

namespace MissionSharedLibrary.Controller
{
    public class DefaultMissionStartingHandlerAdder : ADefaultMissionStartingHandlerAdder
    {
        public DefaultMissionStartingHandlerAdder()
        {
            Global.GetProvider<AMissionStartingManager>().AddHandler(new DefaultMissionStartingHandler());
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
            //MissionStartingManager.AddMissionBehaviour(entranceView,
            //    Global.GetProvider<AInputControllerFactory>().CreateInputController(entranceView.Mission));
            MissionStartingManager.AddMissionBehaviour(entranceView, AMenuManager.Get().CreateMenuView());
            MissionStartingManager.AddMissionBehaviour(entranceView, AMenuManager.Get().CreateGameKeyConfigView());
        }
    }
}
