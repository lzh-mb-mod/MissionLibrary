using MissionLibrary.src.View;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
    public class UsageCategoryViewModel: AUsageCategoryViewModel
    {
        public UsageCategoryViewModel(UsageCategoryData usageCategoryData)
        {
            Name = new TextViewModel(usageCategoryData.Name);
            foreach (var usage in usageCategoryData.UsageList)
            {
                UsageList.Add(new TextViewModel(usage));
            }
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
    }
}