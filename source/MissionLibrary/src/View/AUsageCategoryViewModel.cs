using TaleWorlds.Library;

// The namespace should be MissionLibrary.View. But we have to keep it as is for compatibility.
namespace MissionLibrary.src.View
{
    public abstract class AUsageCategoryViewModel: ViewModel
    {
        public abstract void UpdateSelection(bool isSelected);
    }
}
