﻿using TaleWorlds.Core;
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
        protected IGauntletMovie _movie;

        public bool IsActivated { get; set; }

        protected MissionMenuViewBase(int viewOrderPriority, string movieName, bool pauseGameEngine = true)
        {
            ViewOrderPriority = viewOrderPriority;
            _movieName = movieName;
            _pauseGameEngine = pauseGameEngine;
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
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
            spriteData.SpriteCategories["ui_saveload"]?.Load(resourceContext, uiResourceDepot);
            MissionScreen.AddLayer(GauntletLayer);
            GauntletLayer.IsFocusLayer = true;
            ScreenManager.TrySetFocus(GauntletLayer);
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
                MBCommon.PauseGameEngine();
                Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
            }
            MissionState.Current.Paused = true;
        }

        private void UnpauseGame()
        {
            if (_pauseGameEngine)
            {
                MBCommon.UnPauseGameEngine();
                Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
            }
            MissionState.Current.Paused = false;
        }
    }
}
