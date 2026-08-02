using MissionLibrary.HotKey;
using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace MissionSharedLibrary.View.HotKey
{
    public class GameKeyConfigView : MissionView
    {
        private GauntletLayer _gauntletLayer;
        private GauntletMovieIdentifier _movie;
        private GameKeyConfigVM _dataSource;
        private KeybindingPopup _keybindingPopup;
        private IHotKeySetter _currentGameKey;
        private bool _enableKeyBindingPopupNextTick;
        private string _movieName;

        public const string KeyBindRequestEventId = "KeyBindRequest";
        public const string KeyBindRequestReceiverId = "GameKeyConfigView";

        public GameKeyConfigView(Version version)
        {
            ViewOrderPriority = 50;
            _movieName = "MissionLibraryOptionsGameKeyScreen-" + version;
        }

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            _keybindingPopup = new KeybindingPopup(SetHotKey, MissionScreen);
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            _keybindingPopup.OnToggle(false);
            _keybindingPopup = null;
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (_gauntletLayer == null)
                return;
            if (!_keybindingPopup.IsActive && _gauntletLayer.Input.IsHotKeyReleased("Exit"))
            {
                _dataSource.ExecuteCancel();
            }
            _keybindingPopup.Tick();
            if (_enableKeyBindingPopupNextTick)
            {
                _enableKeyBindingPopupNextTick = false;
                _keybindingPopup.OnToggle(true);
            }
        }

        public void Activate()
        {
            _dataSource = new GameKeyConfigVM(AGameKeyCategoryManager.Get(), OnKeyBindRequest, Deactivate);
            _gauntletLayer = new GauntletLayer(_movieName, ViewOrderPriority);
            _movie = _gauntletLayer.LoadMovie(_movieName, _dataSource);
            _gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _gauntletLayer.InputRestrictions.SetInputRestrictions();
            _gauntletLayer.IsFocusLayer = true;
            MissionScreen.AddLayer(_gauntletLayer);
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        public void Deactivate()
        {
            if (_gauntletLayer == null)
                return;
            _gauntletLayer.InputRestrictions.ResetInputRestrictions();
            if (_movie != null)
            {
                _gauntletLayer.ReleaseMovie(_movie);
            }
            MissionScreen.RemoveLayer(_gauntletLayer);
            _gauntletLayer = null;
            _movie = null;
            _dataSource.OnFinalize();
            _dataSource = null;
        }

        private void OnKeyBindRequest(IHotKeySetter requestedHotKeyToChange)
        {
            _currentGameKey = requestedHotKeyToChange;
            _enableKeyBindingPopupNextTick = true;
        }

        private void SetHotKey(Key key)
        {
            //if (_dataSource.Groups.First<GameKeyGroupVM>((g => g.GameKeys.Contains(this._currentGameKey))).GameKeys.Any<GameKeyOptionVM>(keyVM => keyVM.CurrentKey.InputKey == key.InputKey))
            //    InformationManager.AddQuickInformation(new TextObject("{=n4UUrd1p}Already in use"));
            /*else*/ if (_gauntletLayer.Input.IsHotKeyReleased("Exit"))
            {
                _currentGameKey = null;
                _keybindingPopup.OnToggle(false);
                _dataSource.Update();
            }
            else
            {
                _currentGameKey?.Set(key.InputKey);
                _currentGameKey = null;
                _keybindingPopup.OnToggle(false);
            }
        }

    }
}
