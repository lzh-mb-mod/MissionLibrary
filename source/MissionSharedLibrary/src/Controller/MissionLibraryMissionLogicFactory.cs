﻿using MissionLibrary.Controller;
using MissionSharedLibrary.Controller.MissionBehaviors;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Controller
{
    // Deprecated
    public class MissionLibraryMissionLogicFactory : AMissionLogicFactory
    {
        public override List<MissionLogic> CreateMissionLogics(Mission mission)
        {
            return new List<MissionLogic>
            {
            };
        }
    }
}
