using MissionLibrary.Config.HotKey;
using MissionSharedLibrary.Config.HotKey;

namespace MissionLibrary.HotKey.Category
{
    public enum GeneralGameKey
    {
        OpenMenu,
        NumberOfGameKeyEnums
    }

    public class MissionLibraryGameKeyCategories
    {
        public static IGameKeyCategory GeneralGameKeyCategory { get; set; }

        public static IGameKeyCategory CreateGeneralGameKeyCategory()
        {
            return new GameKeyCategory(nameof(MissionLibrary) + nameof(GeneralGameKey), (int) GeneralGameKey.NumberOfGameKeyEnums,
                MissionLibraryGameKeyConfig.Get());
        }

        public static void RegisterGameKeyCategory()
        {
            GeneralGameKeyCategory = CreateGeneralGameKeyCategory();
            Global.GameKeyCategoryManager.AddCategories(GeneralGameKeyCategory, true);
        }

        public static void Clear()
        {
            GeneralGameKeyCategory = null;
        }
    }
}
