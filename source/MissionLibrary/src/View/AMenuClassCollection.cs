using MissionLibrary.Category;
using MissionLibrary.Provider;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
    public abstract class AMenuClassCollection: ARepository<AMenuClassCollection, AOptionClass>
    {
        public abstract void OnOptionClassSelected(AOptionClass optionClass);

        public abstract void Clear();

        public abstract ViewModel GetViewModel();
    }
}