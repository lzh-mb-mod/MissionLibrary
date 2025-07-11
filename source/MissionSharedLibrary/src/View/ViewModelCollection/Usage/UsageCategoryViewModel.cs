using MissionLibrary.src.View;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
    public class UsageCategoryViewModel: AUsageCategoryViewModel
    {
        private readonly Action _onSelect;
        public UsageCategoryViewModel(UsageCategoryData usageCategoryData, Action onSelect)
        {
            Name = new TextViewModel(usageCategoryData.Name);
            foreach (var usage in usageCategoryData.UsageList)
            {
                UsageList.Add(new TextViewModel(usage));
            }
            _onSelect = onSelect;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            Name.RefreshValues();
            foreach (var viewModel in UsageList)
            {
                viewModel.RefreshValues();
            }
        }

        [DataSourceProperty]
        public TextViewModel Name { get; }

        [DataSourceProperty]
        public MBBindingList<TextViewModel> UsageList { get; } = new MBBindingList<TextViewModel>();

        private bool _isSelected;

        [DataSourceProperty]
        public bool IsSelected
        {
            get => this._isSelected;
            set
            {
                if (value == this._isSelected)
                    return;
                this._isSelected = value;
                this.OnPropertyChangedWithValue((object)value, nameof(IsSelected));
            }
        }
        public override void UpdateSelection(bool isSelected)
        {
            IsSelected = isSelected;
        }

        public void ExecuteSelection()
        {
            _onSelect();
        }
    }
}