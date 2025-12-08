using MissionLibrary.Config.HotKey;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Usage;
using System;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

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

        public static AGameKeyCategory CreateGeneralGameKeyCategory()
        {
            var result = new GameKeyCategory(CategoryId, (int) GeneralGameKey.NumberOfGameKeyEnums,
                GeneralGameKeyConfig.Get());

            result.AddGameKeySequence(new GameKeySequence((int) GeneralGameKey.OpenMenu,
                nameof(GeneralGameKey.OpenMenu),
                CategoryId,
                new List<GameKeySequenceAlternative>()
                {
                    new GameKeySequenceAlternative
                    (
                         new List<InputKey> () {
                            InputKey.L
                        }
                    )
                },
                // mandatory
                true,
                // forbidden keys
                new List<GameKeySequenceAlternative>()
                {
                    new GameKeySequenceAlternative
                    (
                        new List<InputKey> () {
                             InputKey.LeftMouseButton
                        }
                    ),
                    new GameKeySequenceAlternative
                    (
                        new List<InputKey>()
                        {
                            InputKey.RightMouseButton
                        }
                    )
                }));
            return result;
        }

        public static void RegisterGameKeyCategory()
        {
            AGameKeyCategoryManager.Get()?.RegisterGameKeyCategory(CreateGeneralGameKeyCategory, CategoryId, new Version(2, 0));
        }

        public static IGameKeySequence GetKey(GeneralGameKey key)
        {
            return Category.GetGameKeySequence((int) key);
        }
    }
}
