using MissionLibrary.Provider;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace MissionLibrary.Controller
{
    public abstract class AMissionLogicFactory : ATag<AMissionLogicFactory>
    {
        public abstract List<MissionLogic> CreateMissionLogics(Mission mission);
    }
}
