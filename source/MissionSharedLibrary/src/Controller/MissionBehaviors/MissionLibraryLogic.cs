using MissionLibrary.View;
using MissionSharedLibrary.Config;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Controller.MissionBehaviors
{
    public class MissionLibraryLogic : MissionLogic
    {
        private GeneralConfig _config = GeneralConfig.Get();
        private bool _showUsageHintAfterTimeUp = false;
        private float _timerForShowingUsageHint = 0;

        public override void OnTeamDeployed(Team team)
        {
            base.OnTeamDeployed(team);

            if (team == Mission.PlayerTeam)
            {

                if (!_config.HasUsageShown)
                {
                    _config.HasUsageShown = true;
                    _config.Serialize();
                    //_showUsageHintAfterTimeUp = true;
                    //_timerForShowingUsageHint = 2f;
                    AMenuManager.Get()?.RequestToOpenUsageView();
                }
            }
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            if (_showUsageHintAfterTimeUp)
            {
                _timerForShowingUsageHint -= dt;
                if (_timerForShowingUsageHint  < 0f)
                {
                    AMenuManager.Get()?.RequestToOpenUsageView();
                    _showUsageHintAfterTimeUp = false;
                    _timerForShowingUsageHint = 0f;
                }
            }
        }
    }
}
