using MissionLibrary.Controller;
using MissionSharedLibrary.Controller.MissionBehaviors;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.Controller
{

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
        }
    }
}
