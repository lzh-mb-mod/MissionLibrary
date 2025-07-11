using MissionLibrary.Repository;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace MissionLibrary.Usage
{
    public abstract class AUsageCategory : AItem<AUsageCategory>
    {
        public abstract ViewModel ViewModel { get; }

        public abstract void UpdateSelection(bool isSelected);
    }
}
