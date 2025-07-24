using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Utilities;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
    public class MissionLibraryGameKeySequenceOptionVM : AHotKeyConfigVM
    {
        private readonly Action<MissionLibraryGameKeyOptionVM> _onKeybindRequest;
        private readonly string _groupId;
        private readonly string _id;
        private string _name;
        private string _description;
        private MBBindingList<MissionLibraryGameKeySequenceAlternativeOptionVM> _alternatives;
        private bool _pushEnabled;
        private bool _popEnabled;

        public GameKeySequence GameKeySequence { get; private set; }

        public MissionLibraryGameKeySequenceOptionVM(
          GameKeySequence gameKeySequence,
          Action<MissionLibraryGameKeyOptionVM> onKeybindRequest)
        {
            _onKeybindRequest = onKeybindRequest;
            GameKeySequence = gameKeySequence;
            _groupId = GameKeySequence.CategoryId;
            _id = ((GameKeyDefinition)GameKeySequence.Id).ToString();
            UpdateAlternatives();
            RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Name = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", _groupId + "_" + _id).ToString();
            Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_description", _groupId + "_" + _id).ToString();
        }

        public override void Update()
        {
            foreach (var alternative in _alternatives)
            {
                alternative.Update();
            }

            // remove alternatives that was just added but canceled binding.
            var alternativesToRemove = _alternatives.Where(alternative => alternative.Options.Count == 0).ToList();
            foreach (var alternative in alternativesToRemove)
            {
                Alternatives.Remove(alternative);
            }
            UpdateButtons();
        }

        public override void OnDone()
        {
            foreach (var alternative in _alternatives)
            {
                alternative.OnDone();
            }

            GameKeySequence.SetGameKeys(Alternatives.Select(vm => vm.GameKeySequenceAlternative).ToList());
        }

        public override void OnReset()
        {
            GameKeySequence.ResetToDefault();
            
            UpdateAlternatives();
        }

        [DataSourceProperty]
        public bool PushEnabled
        {
            get => _pushEnabled;
            set
            {
                _pushEnabled = value;
                OnPropertyChanged(nameof(PushEnabled));
            }
        }

        public void PushAlternative()
        {
            var alternative = new GameKeySequenceAlternative(new List<InputKey> { InputKey.Invalid});
            var newVM = new MissionLibraryGameKeySequenceAlternativeOptionVM(alternative, _onKeybindRequest);
            Alternatives.Add(newVM);
            UpdateButtons();
            if (newVM.Options.Count < 1)
                return;
            _onKeybindRequest?.Invoke(newVM.Options.First());
        }

        [DataSourceProperty]
        public bool PopEnabled
        {
            get => _popEnabled;
            set
            {
                _popEnabled = value;
                OnPropertyChanged(nameof(PopEnabled));
            }
        }

        public void PopAlternative()
        {
            Alternatives.RemoveAt(Alternatives.Count - 1);
            UpdateButtons();
        }

        public bool IsChanged()
        {
            return _alternatives.Any(option => option.IsChanged());
        }

        private void UpdateButtons()
        {
            PopEnabled = Alternatives.Count > (GameKeySequence.Mandatory ? 1 : 0);
            PushEnabled = Alternatives.Count < 4;
        }

        private void UpdateAlternatives()
        {
            Alternatives = new MBBindingList<MissionLibraryGameKeySequenceAlternativeOptionVM>();
            foreach (var key in GameKeySequence.KeyAlternatives)
            {
                Alternatives.Add(new MissionLibraryGameKeySequenceAlternativeOptionVM(key, _onKeybindRequest));
            }
            UpdateButtons();
        }

        [DataSourceProperty]
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;
                _name = value;
                OnPropertyChangedWithValue(value, nameof(Name));
            }
        }

        [DataSourceProperty]
        public string Description
        {
            get => _description;
            set
            {
                if (_description == value)
                    return;
                _description = value;
                OnPropertyChangedWithValue(value, nameof(Description));
            }
        }

        [DataSourceProperty]
        public MBBindingList<MissionLibraryGameKeySequenceAlternativeOptionVM> Alternatives
        {
            get => _alternatives;
            set
            {
                if (_alternatives == value)
                    return;
                _alternatives = value;
                OnPropertyChanged(nameof(Alternatives));
            }
        }

        [DataSourceProperty]
        public string GameKeySequenceOptionVMVersion { get; private set; } = "v2";

        public TextViewModel AddShortcut { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_add_shortcut"));
        public TextViewModel RemoveShortcut { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_remove_shortcut"));
    }
}
