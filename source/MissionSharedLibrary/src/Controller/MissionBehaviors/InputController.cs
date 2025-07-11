using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Utilities;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Controller.MissionBehaviors
{
    public class InputController : MissionLogic
    {
        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            if (GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed(Mission.InputManager))
            {
                Utility.DisplayMessage("L pressed.");
            }
        }
    }
}
