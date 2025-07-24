using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
    public class MissionLibraryGameKeySequenceAlternativeOptionVM : AHotKeyConfigVM
    {
        private readonly Action<MissionLibraryGameKeyOptionVM> _onKeybindRequest;
        private MBBindingList<MissionLibraryGameKeyOptionVM> _options;
        private bool _pushEnabled;
        private bool _popEnabled;

        public GameKeySequenceAlternative GameKeySequenceAlternative { get; private set; }

        public MissionLibraryGameKeySequenceAlternativeOptionVM(
          GameKeySequenceAlternative gameKeySequenceAlternative,
          Action<MissionLibraryGameKeyOptionVM> onKeybindRequest)
        {
            _onKeybindRequest = onKeybindRequest;
            GameKeySequenceAlternative = gameKeySequenceAlternative;
            UpdateOptions();
            RefreshValues();
        }

        public override void Update()
        {
            //foreach (var option in _options)
            //{
            //    option.Update();
            //}

            // Remove keys that was just added but canceled binding keys
            var optionsToRemove = _options.Where(option => option.CurrentKey.InputKey == InputKey.Invalid).ToList();
            foreach(var option in optionsToRemove)
            {
                Options.Remove(option);
            }
            UpdateButtons();
        }

        public override void OnDone()
        {
            foreach (var option in _options)
            {
                option.OnDone();
            }

            GameKeySequenceAlternative.SetGameKeys(Options.Select(vm => vm.Key.InputKey).ToList());
        }

        public override void OnReset()
        {
            //UpdateOptions();
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

        public void PushGameKey()
        {
            var newVM = new MissionLibraryGameKeyOptionVM(new Key(InputKey.Invalid), _onKeybindRequest);
            Options.Add(newVM);
            UpdateButtons();
            _onKeybindRequest?.Invoke(newVM);
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

        public void PopGameKey()
        {
            Options.RemoveAt(Options.Count - 1);
            UpdateButtons();
        }

        public bool IsChanged()
        {
            return _options.Any(option => option.IsChanged());
        }

        private void UpdateButtons()
        {
            PopEnabled = Options.Count > 1;
            PushEnabled = Options.Count < 4;
        }

        private void UpdateOptions()
        {
            Options = new MBBindingList<MissionLibraryGameKeyOptionVM>();
            foreach (var key in GameKeySequenceAlternative.Keys)
            {
                Options.Add(new MissionLibraryGameKeyOptionVM(key, _onKeybindRequest));
            }
            UpdateButtons();
        }

        [DataSourceProperty]
        public MBBindingList<MissionLibraryGameKeyOptionVM> Options
        {
            get => _options;
            set
            {
                if (_options == value)
                    return;
                _options = value;
                OnPropertyChanged(nameof(Options));
            }
        }

        public TextViewModel AddKey { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_add_key"));
        public TextViewModel RemoveKey { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_remove_key"));
    }
}
