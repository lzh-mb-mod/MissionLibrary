using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace MissionLibrary.HotKey
{
    public interface IGameKeyCategory
    {
        List<GameKey> GameKeys { get; }

        string GameKeyCategoryId { get; }
    }
}
