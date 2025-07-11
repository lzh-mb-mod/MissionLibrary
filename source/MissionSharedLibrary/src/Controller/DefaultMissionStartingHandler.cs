using MissionLibrary;
using MissionLibrary.Controller;
using MissionLibrary.View;
using MissionSharedLibrary.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.Controller
{
    public class DefaultMissionStartingHandlerAdder : ADefaultMissionStartingHandlerAdder
    {
        public DefaultMissionStartingHandlerAdder()
        {
            Global.GetInstance<AMissionStartingManager>().AddHandler(new DefaultMissionStartingHandler());
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
            //MissionStartingManager.AddMissionBehavior(entranceView,
            //    Global.GetProvider<AInputControllerFactory>().CreateInputController(entranceView.Mission));
            MissionStartingManager.AddMissionBehavior(entranceView, AMenuManager.Get().CreateMenuView());
            MissionStartingManager.AddMissionBehavior(entranceView, AMenuManager.Get().CreateGameKeyConfigView());
            MissionStartingManager.AddMissionBehavior(entranceView, new UsageView(26, new System.Version(1, 0, 0)));
        }
    }
}
