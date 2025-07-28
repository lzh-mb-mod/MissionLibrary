using System;
using JetBrains.Annotations;
using MissionLibrary.Config.HotKey;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using System.Collections.Generic;
using TaleWorlds.InputSystem;
using MissionSharedLibrary.Usage;

namespace MissionSharedLibrary.HotKey
{
    public enum GeneralGameKey
    {
        OpenMenu,
        NumberOfGameKeyEnums
    }

    public class GeneralGameKeyCategory
    {
        public const string CategoryId = nameof(MissionLibrary) + nameof(GeneralGameKey);

        public static AGameKeyCategory Category => AGameKeyCategoryManager.Get()?.GetItem(CategoryId);

        [NotNull]
        public static AGameKeyCategory CreateGeneralGameKeyCategory()
        {
            var result = new GameKeyCategory(CategoryId, (int) GeneralGameKey.NumberOfGameKeyEnums,
                GeneralGameKeyConfig.Get());

            result.AddGameKeySequence(new GameKeySequence((int) GeneralGameKey.OpenMenu,
                nameof(GeneralGameKey.OpenMenu),
                CategoryId, new List<GameKeySequenceAlternative>()
                {
                    new GameKeySequenceAlternative
                    (
                         new List<InputKey> () {
                            InputKey.L
                        }
                    )
                }, true));
            return result;
        }

        public static void RegisterGameKeyCategory()
        {
            AGameKeyCategoryManager.Get()?.RegisterItem(CreateGeneralGameKeyCategory, CategoryId, new Version(1, 2));
        }

        public static IGameKeySequence GetKey(GeneralGameKey key)
        {
            return Category.GetGameKeySequence((int) key);
        }
    }
}
