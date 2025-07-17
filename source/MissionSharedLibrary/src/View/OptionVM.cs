using MissionLibrary.View;
using MissionSharedLibrary.View.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View
{
    public class OptionVM : MissionMenuVMBase
    {
        private readonly AMenuClassCollection _menuClassCollection;

        public OptionVM(AMenuClassCollection menuClassCollection, Action closeMenu)
            : base(closeMenu)
        {
            _menuClassCollection = menuClassCollection;
            OptionClassCollection = menuClassCollection.GetViewModel();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            ConfigKeyTitle.RefreshValues();
            ConfigKeyHint.RefreshValues();
            ShowUsageTitle.RefreshValues();
            ShowUsageHint.RefreshValues();
            OptionClassCollection.RefreshValues();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            _menuClassCollection.Clear();
        }

        public TextViewModel ConfigKeyTitle { get; set; } =
            new TextViewModel(GameTexts.FindText("str_mission_library_gamekey_config"));

        public HintViewModel ConfigKeyHint { get; set; } =
            new HintViewModel(GameTexts.FindText("str_mission_library_config_key_hint"));

        private readonly GameKeyConfigView _gameKeyConfigView = Mission.Current.GetMissionBehavior<GameKeyConfigView>();

        public void ConfigKey()
        {
            InformationManager.HideInquiry();
            _gameKeyConfigView?.Activate();
        }

        public TextViewModel ShowUsageTitle { get; set; } =
            new TextViewModel(GameTexts.FindText("str_mission_library_show_usage"));

        public HintViewModel ShowUsageHint { get; set; } =
            new HintViewModel(GameTexts.FindText("str_mission_library_show_usage_hint"));

        public void ShowUsageView()
        {
            Mission.Current?.GetMissionBehavior<UsageView>()?.ActivateMenu();
        }

        public ViewModel OptionClassCollection { get; }
    }
}
