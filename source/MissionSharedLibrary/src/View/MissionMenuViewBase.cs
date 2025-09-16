using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace MissionSharedLibrary.View
{
    public abstract class MissionMenuViewBase : MissionView
    {
        protected readonly string _movieName;
        private readonly bool _pauseGameEngine;
        protected MissionMenuVMBase DataSource;
        protected GauntletLayer GauntletLayer;
        protected GauntletMovieIdentifier _movie;
        protected bool _enginePausedBySelf;
        protected bool _missionPausedBySelf;
        protected bool _focus;

        public bool IsActivated { get; set; }

        protected MissionMenuViewBase(int viewOrderPriority, string movieName, bool pauseGameEngine = true, bool focus = true)
        {
            ViewOrderPriority = viewOrderPriority;
            _movieName = movieName;
            _pauseGameEngine = pauseGameEngine;
            _focus = focus;
        }

        protected abstract MissionMenuVMBase GetDataSource();

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            GauntletLayer = null;
            DataSource?.OnFinalize();
            DataSource = null;
            _movie = null;
        }

        public void ToggleMenu()
        {
            if (IsActivated)
                DeactivateMenu();
            else
                ActivateMenu();
        }

        public virtual void ActivateMenu()
        {
            if (IsActivated)
                return;
            IsActivated = true;
            DataSource = GetDataSource();
            if (DataSource == null)
                return;
            GauntletLayer = new GauntletLayer(ViewOrderPriority);
            GauntletLayer.InputRestrictions.SetInputRestrictions();
            GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _movie = GauntletLayer.LoadMovie(_movieName, DataSource);
            SpriteData spriteData = UIResourceManager.SpriteData;
            UIResourceManager.LoadSpriteCategory("ui_saveload");
            MissionScreen.AddLayer(GauntletLayer);
            if (_focus)
            {
                GauntletLayer.IsFocusLayer = true;
                ScreenManager.TrySetFocus(GauntletLayer);
            }
            PauseGame();
        }

        public virtual void DeactivateMenu()
        {
            if (!IsActivated)
                return;
            DataSource?.CloseMenu();
        }

        protected void OnCloseMenu()
        {
            IsActivated = false;
            GauntletLayer.InputRestrictions.ResetInputRestrictions();
            GauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(GauntletLayer);
            MissionScreen.RemoveLayer(GauntletLayer);
            DataSource.OnFinalize();
            DataSource = null;
            _movie = null;
            GauntletLayer = null;
            UnpauseGame();
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);
            if (IsActivated)
            {
                if (GauntletLayer.Input.IsKeyReleased(InputKey.RightMouseButton) ||
                    GauntletLayer.Input.IsHotKeyReleased("Exit") || GauntletLayer.Input.IsHotKeyReleased("ToggleEscapeMenu"))
                    DeactivateMenu();
            }
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            UnpauseGame();
        }

        private void PauseGame()
        {
            if (_pauseGameEngine)
            {
                if (!MBCommon.IsPaused)
                {
                    _enginePausedBySelf = true;
                    MBCommon.PauseGameEngine();
                    Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
                }
            }
            if (!MissionState.Current.Paused)
            {
                _missionPausedBySelf = true;
                MissionState.Current.Paused = true;
            }
        }

        private void UnpauseGame()
        {
            if (_pauseGameEngine)
            {
                if (_enginePausedBySelf)
                {
                    _enginePausedBySelf = false;
                    MBCommon.UnPauseGameEngine();
                    Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
                }
            }
            if (_missionPausedBySelf)
            {
                _missionPausedBySelf = false;
                MissionState.Current.Paused = false;
            }
        }
    }
}
