using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Utilities;
using MissionSharedLibrary.View.ViewModelCollection.HotKey;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionSharedLibrary.HotKey
{
    public class GameKeyCategory : AGameKeyCategory
    {
        public List<GameKeySequence> GameKeySequences { get; }

        public override string ItemId { get; }

        private readonly IGameKeyConfig _config;

        public GameKeySequence GetKeySequence(int i)
        {
            if (GameKeySequences == null || i < 0 || i >= GameKeySequences.Count)
            {
                return new GameKeySequence(0, "", "", new List<GameKeySequenceAlternative>());
            }

            return GameKeySequences[i];
        }

        public override IGameKeySequence GetGameKeySequence(int i)
        {
            return GetKeySequence(i);
        }

        public SerializedGameKeyCategory ToSerializedGameKeyCategory()
        {
            return new SerializedGameKeyCategory
            {
                CategoryId = ItemId,
                GameKeySequences = GameKeySequences.Select(sequence => sequence.ToSerializedGameKeySequence()).ToList()
            };
        }

        public void FromSerializedGameKeyCategory(SerializedGameKeyCategory category)
        {
            var dictionary = category.GameKeySequences.Select(gameKeySequence => gameKeySequence.StringId).Distinct()
                .Select(stringId => category.GameKeySequences.First(gameKeySequence => gameKeySequence.StringId == stringId)).ToDictionary(serializedGameKey => serializedGameKey.StringId);
            for (var i = 0; i < GameKeySequences.Count; i++)
            {
                var gameKeySequence = GameKeySequences[i];
                if (dictionary.TryGetValue(gameKeySequence.StringId, out SerializedGameKeySequence serializedGameKeySequence))
                {
                    GameKeySequences[i].SetGameKeys(serializedGameKeySequence.GameKeyAlternatives.Select(sa => new GameKeySequenceAlternative(sa.KeyboardKeys)).ToList());
                }
            }
        }

        public override void Save()
        {
            try
            {
                _config.Category = ToSerializedGameKeyCategory();
                _config.Serialize();
            }
            catch (Exception e)
            {
                Utility.DisplayMessage(e.ToString());
                Console.WriteLine(e);
            }
        }

        public override void Load()
        {
            try
            {
                //_config.Deserialize();
                FromSerializedGameKeyCategory(_config.Category);
            }
            catch (Exception e)
            {
                Utility.DisplayMessage(e.ToString());
                Console.WriteLine(e);
            }
        }

        public GameKeyCategory(string categoryId, int gameKeysCount, IGameKeyConfig config)
        {
            ItemId = categoryId;
            _config = config;

            GameKeySequences = new List<GameKeySequence>(gameKeysCount);
            for (int index = 0; index < gameKeysCount; ++index)
                GameKeySequences.Add(null);
        }

        public void AddGameKeySequence(GameKeySequence gameKeySequence)
        {
            if (gameKeySequence.Id < 0 || gameKeySequence.Id >= GameKeySequences.Count)
                return;

            GameKeySequences[gameKeySequence.Id] = gameKeySequence;
        }

        public override AHotKeyConfigVM CreateViewModel(Action<IHotKeySetter> onKeyBindRequest)
        {
            return new MissionLibraryGameKeySequenceGroupVM(ItemId, GameKeySequences, onKeyBindRequest);
        }
    }
}
