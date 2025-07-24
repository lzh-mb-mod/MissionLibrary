﻿using MissionLibrary.HotKey;
using System;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
    public class MissionLibraryGameKeyOptionVM : ViewModel, IHotKeySetter
    {
        private readonly Action<MissionLibraryGameKeyOptionVM> _onKeybindRequest;
        private string _optionValueText;

        public Key CurrentKey { get; private set; }

        public Key Key { get; private set; }

        public MissionLibraryGameKeyOptionVM(
          Key key,
          Action<MissionLibraryGameKeyOptionVM> onKeybindRequest)
        {
            _onKeybindRequest = onKeybindRequest;
            Key = key;
            CurrentKey = new Key(Key.InputKey);
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", CurrentKey.ToString().ToLower()).ToString();
        }

        private void ExecuteKeybindRequest() => _onKeybindRequest(this);

        public void Set(InputKey newKey) => OnKeySet(newKey);

        private void OnKeySet(InputKey key)
        {
            CurrentKey.ChangeKey(key);
            OptionValueText = Module.CurrentModule.GlobalTextManager
                .FindText("str_game_key_text", CurrentKey.ToString().ToLower()).ToString();
        }

        public void Update()
        {
            CurrentKey = new Key(Key.InputKey);
            OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", CurrentKey.ToString().ToLower()).ToString();
        }

        public void OnDone() => Key.ChangeKey(CurrentKey.InputKey);

        internal bool IsChanged() => CurrentKey.InputKey != Key.InputKey;

        [DataSourceProperty]
        public string OptionValueText
        {
            get => _optionValueText;
            set
            {
                if (_optionValueText == value)
                    return;
                _optionValueText = value;
                OnPropertyChangedWithValue(value, nameof(OptionValueText));
            }
        }
    }
}
