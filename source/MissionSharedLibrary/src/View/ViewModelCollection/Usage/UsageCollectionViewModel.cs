using MissionLibrary.Usage;
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
        private UsageCategoryContainerViewModel _currentUsageCategoryContainerViewModel;
        private AUsageCategoryManager _usageCategoryManager;
        private readonly Action _onClose;
        public UsageCollectionViewModel(TextObject title, AUsageCategoryManager usageCategoryManager, Action onClose)
        {
            Title = new TextViewModel(title);
            _usageCategoryManager = usageCategoryManager;
            _onClose = onClose;
            foreach (var usageCategory in _usageCategoryManager.Items.Values.Select(v => v.Value))
            {
                UsageCategoryContainerViewModels.Add(new UsageCategoryContainerViewModel(usageCategory, OnUsageCategorySelected));
            }
            OnUsageCategorySelected(
                 UsageCategoryContainerViewModels.FirstOrDefault());
        }

        public void OnUsageCategorySelected(UsageCategoryContainerViewModel usageCategoryViewModel)
        {
            if (CurrentUsageCategoryContainerViewModel == usageCategoryViewModel)
                return;
            CurrentUsageCategoryContainerViewModel?.UpdateSelection(false);
            CurrentUsageCategoryContainerViewModel = usageCategoryViewModel;
            CurrentUsageCategoryContainerViewModel?.UpdateSelection(true);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            Title.RefreshValues();
            foreach (var viewModel in UsageCategoryContainerViewModels)
            {
                viewModel.RefreshValues();
            }

            CurrentUsageCategoryContainerViewModel?.RefreshValues();
        }

        public void OnNext()
        {
            var index = UsageCategoryContainerViewModels.FindIndex(viewModel => viewModel == CurrentUsageCategoryContainerViewModel);
            if (index == -1)
            {
                OnUsageCategorySelected(
                     UsageCategoryContainerViewModels.FirstOrDefault());
            }
            else if (index == UsageCategoryContainerViewModels.Count - 1)
            {
                _onClose?.Invoke();
            }
            else
            {
                OnUsageCategorySelected(UsageCategoryContainerViewModels[index + 1]);
            }
        }

        [DataSourceProperty]
        public TextViewModel Title { get; }

        [DataSourceProperty]
        public MBBindingList<UsageCategoryContainerViewModel> UsageCategoryContainerViewModels { get; } = new MBBindingList<UsageCategoryContainerViewModel>();

        [DataSourceProperty]
        public UsageCategoryContainerViewModel CurrentUsageCategoryContainerViewModel
        {
            get => _currentUsageCategoryContainerViewModel;
            set
            {
                if (_currentUsageCategoryContainerViewModel == value)
                    return;
                _currentUsageCategoryContainerViewModel = value;
                OnPropertyChanged(nameof(CurrentUsageCategoryContainerViewModel));
            }
        }
    }
}