using MissionLibrary.Config.HotKey;
using MissionSharedLibrary.Config.HotKey;
using TaleWorlds.InputSystem;

namespace MissionLibrary.HotKey.Category
{
    public enum GeneralGameKey
    {
        OpenMenu,
        NumberOfGameKeyEnums
    }

    public class MissionLibraryGameKeyCategory
    {
        public const string CategoryId = nameof(MissionLibrary) + nameof(GeneralGameKey);

        public static IGameKeyCategory GeneralGameKeyCategory { get; set; }

        public static IGameKeyCategory CreateGeneralGameKeyCategory()
        {
            var result = new GameKeyCategory(nameof(MissionLibrary) + nameof(GeneralGameKey), (int) GeneralGameKey.NumberOfGameKeyEnums,
                MissionLibraryGameKeyConfig.Get());
            result.AddGameKey(new GameKey((int) GeneralGameKey.OpenMenu, CategoryId + nameof(GeneralGameKey.OpenMenu),
                CategoryId, InputKey.L, CategoryId));
            return result;
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
