using MissionLibrary.HotKey;
using System.Collections.Generic;
using TaleWorlds.InputSystem;

namespace MissionSharedLibrary.Config.HotKey
{
    public class GameKeyCategory : IGameKeyCategory
    {
        public List<GameKey> GameKeys { get; }

        public string GameKeyCategoryId { get; private set; }

        private readonly IGameKeyConfig _config;

        public GameKey GetGameKey(int i)
        {
            if (GameKeys == null || i < 0 || i >= GameKeys.Count)
            {
                return InvalidGameKey();
            }

            return GameKeys[i] ?? InvalidGameKey();
        }

        public void Save()
        {
            _config.Category = SerializedGameKeyCategory.FromGameKeyCategory(this);
            _config.Serialize();
        }

        public void Load()
        {
            _config.Deserialize();
            _config.Category.ToGameKeyCategory(this);
        }

        public GameKeyCategory(string categoryId, int gameKeysCount, IGameKeyConfig config)
        {
            GameKeyCategoryId = categoryId;
            _config = config;
            GameKeys = new List<GameKey>(gameKeysCount);
            for (int index = 0; index < gameKeysCount; ++index)
                GameKeys.Add((GameKey)null);
        }

        public void AddGameKey(GameKey gameKey)
        {
            if (gameKey.Id < 0 || gameKey.Id >= GameKeys.Count)
                return;

            GameKeys[gameKey.Id] = gameKey;
        }

        private GameKey InvalidGameKey()
        {
            return new GameKey(-1, "", "", InputKey.Invalid, "");
        }
    }
}
