using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Utilities;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
    public class MissionLibraryGameKeyOptionCategoryVM : ViewModel
    {
        private readonly AGameKeyCategoryManager _gameKeyCategoryManager;
        private readonly Dictionary<GameKey, InputKey> _keysToChangeOnDone = new Dictionary<GameKey, InputKey>();
        private string _name;
        private MBBindingList<MissionLibraryGameKeyGroupVM> _groups;
        private readonly Dictionary<string, AGameKeyCategory> _categories;

        public MissionLibraryGameKeyOptionCategoryVM(AGameKeyCategoryManager gameKeyCategoryManager, Action<GameKeyOptionVM> onKeyBindRequest)
        {
            _gameKeyCategoryManager = gameKeyCategoryManager;
            _categories = _gameKeyCategoryManager.Categories.ToDictionary(pair => pair.Key, pair => pair.Value.Value);
            Groups = new MBBindingList<MissionLibraryGameKeyGroupVM>();
            foreach (KeyValuePair<string, AGameKeyCategory> category in _categories)
            {
                if (category.Value.GameKeys.Count > 0)
                    Groups.Add(new MissionLibraryGameKeyGroupVM(category.Key, category.Value.GameKeys, onKeyBindRequest, UpdateKeysOfGameKeysWithId));
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
            try
            {
                foreach (var group in this.Groups)
                    group.OnReset();
                _keysToChangeOnDone.Clear();
            }
            catch (Exception e)
            {
                Utility.DisplayMessageForced(e.ToString());
            }
        }

        public void OnDone()
        {
            foreach (MissionLibraryGameKeyGroupVM group in Groups)
                group.OnDone();
            foreach (KeyValuePair<GameKey, InputKey> keyValuePair in _keysToChangeOnDone)
                FindValidInputKey(keyValuePair.Key).ChangeKey(keyValuePair.Value);
            _gameKeyCategoryManager.Save();
        }

        private Key FindValidInputKey(GameKey gameKey)
        {
            return gameKey.KeyboardKey;
        }

        private void UpdateKeysOfGameKeysWithId(string categoryId, int gameKeyId, InputKey newKey)
        {
            if (_categories.TryGetValue(categoryId, out AGameKeyCategory category))
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
