using MissionLibrary.Usage;
using MissionLibrary.View;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System;
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
        private AUsageCategoryManager _usageCategoryManager;
        private readonly Action _onClose;
        private List<AUsageCategory> _usageCategories;
        public UsageCollectionViewModel(TextObject title, AUsageCategoryManager usageCategoryManager, Action onClose)
        {
            Title = new TextViewModel(title);
            _usageCategoryManager = usageCategoryManager;
            _onClose = onClose;
            _usageCategories = _usageCategoryManager.Items.Values.Select(v => v.Value).ToList();
            foreach (var usageCategory in _usageCategories)
            {
                UsageCategoriesViewModel.Add(usageCategory.ViewModel);
            }
            OnUsageCategorySelected(
                 _usageCategories.FirstOrDefault());
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
            foreach (var viewModel in UsageCategoriesViewModel)
            {
                viewModel.RefreshValues();
            }

            CurrentUsageCategoryViewModel?.RefreshValues();
        }

        public void OnNext()
        {
            var index = _usageCategories.FindIndex(category => category == CurrentSelectedUsageCategory);
            if (index == -1)
            {
                OnUsageCategorySelected(
                     _usageCategories.FirstOrDefault());
            }
            else if (index == _usageCategories.Count - 1)
            {
                _onClose?.Invoke();
            }
            else
            {
                OnUsageCategorySelected(_usageCategories[index + 1]);
            }
        }

        [DataSourceProperty]
        public TextViewModel Title { get; }

        [DataSourceProperty]
        public MBBindingList<ViewModel> UsageCategoriesViewModel { get; } = new MBBindingList<ViewModel>();
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