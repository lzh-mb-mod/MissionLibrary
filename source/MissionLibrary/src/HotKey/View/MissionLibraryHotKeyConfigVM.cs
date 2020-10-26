using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace MissionLibrary.HotKey.View
{
    public class MissionLibraryHotKeyConfigVM : ViewModel
    {
        private readonly GameKeyCategoryManager _gameKeyCategoryManager;
        private readonly Dictionary<GameKey, InputKey> _keysToChangeOnDone = new Dictionary<GameKey, InputKey>();
        private string _name;
        private MBBindingList<MissionLibraryGameKeyGroupVM> _groups;

        public MissionLibraryHotKeyConfigVM(GameKeyCategoryManager gameKeyCategoryManager, Action<GameKeyOptionVM> onKeyBindRequest)
        {
            _gameKeyCategoryManager = gameKeyCategoryManager;
            var categories = _gameKeyCategoryManager.Categories.ToDictionary(pair => pair.Key, pair => pair.Value.GameKeys);
            Groups = new MBBindingList<MissionLibraryGameKeyGroupVM>();
            foreach (KeyValuePair<string, List<GameKey>> category in categories)
            {
                if (category.Value.Count > 0)
                    Groups.Add(new MissionLibraryGameKeyGroupVM(category.Key, category.Value, onKeyBindRequest, UpdateKeysOfGameKeysWithId));
            }
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Name = new TextObject("{=Met1U45t}Mouse and Keyboard").ToString();
            Groups.ApplyActionOnAllItems(x => x.RefreshValues());
        }

        public void OnReset()
        {
            foreach (MissionLibraryGameKeyGroupVM group in Groups)
                group.OnReset();
            _keysToChangeOnDone.Clear();
        }

        public void OnDone()
        {
            foreach (MissionLibraryGameKeyGroupVM group in Groups)
                group.OnDone();
            foreach (KeyValuePair<GameKey, InputKey> keyValuePair in _keysToChangeOnDone)
                FindValidInputKey(keyValuePair.Key).ChangeKey(keyValuePair.Value);
        }

        private Key FindValidInputKey(GameKey gameKey)
        {
            return gameKey.PrimaryKey;
        }

        private void UpdateKeysOfGameKeysWithId(string categoryId, int gameKeyId, InputKey newKey)
        {
            if (_gameKeyCategoryManager.Categories.TryGetValue(categoryId, out IGameKeyCategory category))
            {
                if (gameKeyId < 0 || gameKeyId >= category.GameKeys.Count)
                    return;
                var gameKey = category.GameKeys[gameKeyId];
                if (_keysToChangeOnDone.ContainsKey(gameKey))
                    _keysToChangeOnDone[gameKey] = newKey;
                else
                    _keysToChangeOnDone.Add(gameKey, newKey);
            }
        }

        [DataSourceProperty]
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChangedWithValue(value, nameof(Name));
            }
        }

        [DataSourceProperty]
        public MBBindingList<MissionLibraryGameKeyGroupVM> Groups
        {
            get => _groups;
            set
            {
                if (value == _groups)
                    return;
                _groups = value;
                OnPropertyChangedWithValue(value, nameof(Groups));
            }
        }
    }
}
