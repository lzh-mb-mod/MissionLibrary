using MissionLibrary.Category;
using MissionLibrary.Provider;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
    public abstract class AOptionClass : AItem<AOptionClass>, IViewModelProvider<ViewModel>
    {
        public abstract ViewModel GetViewModel();

        public abstract void UpdateSelection(bool isSelected);
    }
}
