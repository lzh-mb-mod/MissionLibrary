using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace MissionLibrary.src.View
{
    public abstract class AUsageCategoryViewModel: ViewModel
    {
        public abstract void UpdateSelection(bool isSelected);
    }
}
