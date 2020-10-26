using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.InputSystem;

namespace MissionLibrary.HotKey.Category
{
    public enum GeneralHotKey
    {
        OpenMenu,
        NumberOfGameKeyEnums
    }

    public class MissionLibraryGeneralHotKeyCategory : IGameKeyCategory
    {
        public const string CategoryId = "MissionLibraryGeneral";
        public List<GameKey> GameKeys { get; }
        public string GameKeyCategoryId => CategoryId;

        public MissionLibraryGeneralHotKeyCategory()
        {
            GameKeys = new List<GameKey>()
            {
                new GameKey((int)GeneralHotKey.OpenMenu, nameof(GeneralHotKey.OpenMenu), CategoryId, InputKey.L, CategoryId)
            };
        }
    }
}
