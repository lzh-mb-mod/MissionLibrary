using MissionLibrary.Controller;
using MissionSharedLibrary.Controller.MissionBehaviors;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Controller
{
    public class InputControllerFactory : AInputControllerFactory
    {
        public override MissionLogic CreateMissionBehaviors(Mission mission)
        {
            return new MissionLibraryLogic();
        }
    }
}
