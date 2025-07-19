using MissionLibrary.src.View;
using MissionLibrary.Usage;
using System;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
    public class UsageCategoryContainerViewModel : AUsageCategoryViewModel
    {
        private readonly AUsageCategory _usageCategory;
        private readonly Action<UsageCategoryContainerViewModel> _onSelect;
        public UsageCategoryContainerViewModel(AUsageCategory usageCategory, Action<UsageCategoryContainerViewModel> onSelect)
        {
            _usageCategory = usageCategory;
            UsageCategoryViewModel = usageCategory.ViewModel;
            _onSelect = onSelect;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            UsageCategoryViewModel.RefreshValues();
        }

        private bool _isSelected;

        public ViewModel UsageCategoryViewModel { get; }

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
            _usageCategory.UpdateSelection(isSelected);
        }

        public void ExecuteSelection()
        {
            _onSelect?.Invoke(this);
        }
    }
}