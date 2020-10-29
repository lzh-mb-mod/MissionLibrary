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
            var result = new GameKeyCategory(CategoryId, (int) GeneralGameKey.NumberOfGameKeyEnums,
                MissionLibraryGameKeyConfig.Get());
            result.AddGameKey(new GameKey((int) GeneralGameKey.OpenMenu, nameof(GeneralGameKey.OpenMenu),
                CategoryId, InputKey.L, CategoryId));
            return result;
        }

        public static void RegisterGameKeyCategory()
        {
            GeneralGameKeyCategory = CreateGeneralGameKeyCategory();
            Global.GameKeyCategoryManager.AddCategories(GeneralGameKeyCategory, true);
        }

        public static InputKey GetKey(GeneralGameKey key)
        {
            return GeneralGameKeyCategory?.GetKey((int) key) ?? InputKey.Invalid;
        }

        public static void Clear()
        {
            GeneralGameKeyCategory = null;
        }
    }
}
