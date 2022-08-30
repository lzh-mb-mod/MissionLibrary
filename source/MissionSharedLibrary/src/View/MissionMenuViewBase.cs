﻿using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.TwoDimension;

namespace MissionSharedLibrary.View
{
    public abstract class MissionMenuViewBase : MissionView
    {
        private readonly string _movieName;
        protected MissionMenuVMBase DataSource;
        protected GauntletLayer GauntletLayer;
        private IGauntletMovie _movie;
        private bool _oldGameStatusDisabledStatus;

        public bool IsActivated { get; set; }

        protected MissionMenuViewBase(int viewOrderPriority, string movieName)
        {
            ViewOrderPriority = viewOrderPriority;
            _movieName = movieName;
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

        public void ActivateMenu()
        {
            IsActivated = true;
            DataSource = GetDataSource();
            if (DataSource == null)
                return;
            GauntletLayer = new GauntletLayer(ViewOrderPriority) { IsFocusLayer = true };
            GauntletLayer.InputRestrictions.SetInputRestrictions();
            GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
            _movie = GauntletLayer.LoadMovie(_movieName, DataSource);
            SpriteData spriteData = UIResourceManager.SpriteData;
            TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
            ResourceDepot uiResourceDepot = UIResourceManager.UIResourceDepot;
            spriteData.SpriteCategories["ui_saveload"]?.Load(resourceContext, uiResourceDepot);
            MissionScreen.AddLayer(GauntletLayer);
            ScreenManager.TrySetFocus(GauntletLayer);
            PauseGame();
        }

        public void DeactivateMenu()
        {
            DataSource?.CloseMenu();
        }
        protected void OnCloseMenu()
        {
            IsActivated = false;
            GauntletLayer.InputRestrictions.ResetInputRestrictions();
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
                    GauntletLayer.Input.IsHotKeyReleased("Exit"))
                    DeactivateMenu();
            }
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            Game.Current.GameStateManager.ActiveStateDisabledByUser = false;
        }

        private void PauseGame()
        {
            MBCommon.PauseGameEngine();
            _oldGameStatusDisabledStatus = Game.Current.GameStateManager.ActiveStateDisabledByUser;
            Game.Current.GameStateManager.ActiveStateDisabledByUser = true;
        }

        private void UnpauseGame()
        {
            MBCommon.UnPauseGameEngine();
            Game.Current.GameStateManager.ActiveStateDisabledByUser = _oldGameStatusDisabledStatus;
        }
    }
}
