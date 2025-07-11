using MissionLibrary.Usage;
using MissionLibrary.View;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
    public class UsageCollectionViewModel: ViewModel
    {
        private AUsageCategory _currentSelectedUsageCategory;
        private ViewModel _currentUsageCategoryViewModel;
        public UsageCollectionViewModel(TextObject title, List<AUsageCategory> usageCategories)
        {
            Title = new TextViewModel(title);
            foreach (var usageCategory in usageCategories)
            {
                UsageCategories.Add(usageCategory.ViewModel);
            }
            OnUsageCategorySelected(
                 usageCategories.FirstOrDefault());
        }

        public void OnUsageCategorySelected(AUsageCategory UsageCategory)
        {
            if (CurrentSelectedUsageCategory == UsageCategory)
                return;
            CurrentSelectedUsageCategory?.UpdateSelection(false);
            CurrentSelectedUsageCategory = UsageCategory;
            CurrentSelectedUsageCategory?.UpdateSelection(true);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            Title.RefreshValues();
            foreach (var viewModel in UsageCategories)
            {
                viewModel.RefreshValues();
            }

            CurrentUsageCategoryViewModel?.RefreshValues();
        }

        [DataSourceProperty]
        public TextViewModel Title { get; }

        [DataSourceProperty]
        public MBBindingList<ViewModel> UsageCategories { get; } = new MBBindingList<ViewModel>();
        public AUsageCategory CurrentSelectedUsageCategory
        {
            get => _currentSelectedUsageCategory;
            private set
            {
                if (_currentSelectedUsageCategory == value)
                    return;
                _currentSelectedUsageCategory = value;
                CurrentUsageCategoryViewModel = _currentSelectedUsageCategory?.ViewModel;
            }
        }

        [DataSourceProperty]
        public ViewModel CurrentUsageCategoryViewModel
        {
            get => _currentUsageCategoryViewModel;
            set
            {
                if (_currentUsageCategoryViewModel == value)
                    return;
                _currentUsageCategoryViewModel = value;
                OnPropertyChanged(nameof(CurrentUsageCategoryViewModel));
            }
        }
    }
}