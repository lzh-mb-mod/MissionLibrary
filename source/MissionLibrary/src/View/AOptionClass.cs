using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionLibrary.Provider;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionLibrary.View
{
    public abstract class AOptionClass : ATag<AOptionClass>, IViewModelProvider<ViewModel>
    {
        public abstract ViewModel GetViewModel();

        public abstract void UpdateSelection(bool isSelected);
    }
}
