using MissionLibrary.Provider;
using MissionLibrary.View;
using MissionSharedLibrary.Category;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection
{
    public class MenuClassCollection : AMenuClassCollection
    {
        private RepositoryImplementation<AOptionClass> _repositoryImplementation = new RepositoryImplementation<AOptionClass>();
        private MenuClassCollectionViewModel _viewModel;
        private GeneralConfig _config = GeneralConfig.Get();

        public override Dictionary<string, IProvider<AOptionClass>> Items => _repositoryImplementation.Items;

        public override void RegisterItem(IProvider<AOptionClass> category, bool addOnlyWhenMissing = true)
        {
            _repositoryImplementation.RegisterItem(category, addOnlyWhenMissing);
        }

        public override AOptionClass GetItem(string categoryId)
        {
            return _repositoryImplementation.GetItem(categoryId);
        }

        public override T GetItem<T>(string categoryId)
        {
            return _repositoryImplementation.GetItem<T>(categoryId);
        }

        public override void OnOptionClassSelected(AOptionClass optionClass)
        {
            _config.PreviouslySelectedOptionClassId = optionClass.ItemId;
            _viewModel?.OnOptionClassSelected(optionClass);
            _config.Serialize();
        }

        public override void Clear()
        {
            _viewModel = null;
            foreach (var optionClass in Items)
            {
                optionClass.Value.Clear();
            }
        }

        public override ViewModel GetViewModel()
        {
            return _viewModel ??= new MenuClassCollectionViewModel(Items.Select(p => p.Value).ToList(), _config.PreviouslySelectedOptionClassId);
        }
    }

    public class MenuClassCollectionViewModel : ViewModel
    {
        private MBBindingList<ViewModel> _optionClassViewModels;
        private AOptionClass _currentSelectedOptionClass;
        private ViewModel _currentOptionClassViewModel;

        public AOptionClass CurrentSelectedOptionClass
        {
            get => _currentSelectedOptionClass;
            private set
            {
                if (_currentSelectedOptionClass == value)
                    return;
                _currentSelectedOptionClass = value;
                CurrentOptionClassViewModel = _currentSelectedOptionClass?.GetViewModel();
            }
        }

        [DataSourceProperty]
        public MBBindingList<ViewModel> OptionClassViewModels
        {
            get => _optionClassViewModels;
            set
            {
                if (_optionClassViewModels == value)
                    return;
                _optionClassViewModels = value;
                OnPropertyChanged(nameof(OptionClassViewModels));
            }
        }

        [DataSourceProperty]
        public ViewModel CurrentOptionClassViewModel
        {
            get => _currentOptionClassViewModel;
            set
            {
                if (_currentOptionClassViewModel == value)
                    return;
                _currentOptionClassViewModel = value;
                OnPropertyChanged(nameof(CurrentOptionClassViewModel));
            }
        }

        public void OnOptionClassSelected(AOptionClass optionClass)
        {
            if (CurrentSelectedOptionClass == optionClass)
                return;
            CurrentSelectedOptionClass?.UpdateSelection(false);
            CurrentSelectedOptionClass = optionClass;
            CurrentSelectedOptionClass?.UpdateSelection(true);
        }

        public MenuClassCollectionViewModel(List<IProvider<AOptionClass>> optionClasses, string selectedOptionClassId)
        {
            var optionClassViewModels = new MBBindingList<ViewModel>();
            foreach (var optionClass in optionClasses)
            {
                try
                {
                    optionClassViewModels.Add(optionClass.Value.GetViewModel());
                }
                catch (Exception e)
                {
                    Utility.DisplayMessageForced(e.ToString());
                    Console.WriteLine(e);
                }
            }
            OptionClassViewModels = optionClassViewModels;
            try
            {
                OnOptionClassSelected(
                    (optionClasses.FirstOrDefault(optionClass => optionClass.Value.ItemId == selectedOptionClassId) ??
                     optionClasses.FirstOrDefault())?.Value);
            }
            catch (Exception e)
            {
                Utility.DisplayMessageForced(e.ToString());
                Console.WriteLine(e);
            }
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            Refresh();
        }

        private void Refresh()
        {
            foreach (var optionClassViewModel in OptionClassViewModels)
            {
                optionClassViewModel.RefreshValues();
            }
        }
    }
}
