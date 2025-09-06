using MissionLibrary.Provider;
using MissionLibrary.Usage;
using MissionSharedLibrary.Category;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace MissionSharedLibrary.Usage
{
    public class UsageCategoryManager : AUsageCategoryManager
    {
        private UsageCollectionViewModel _viewModel;

        private RepositoryImplementation<AUsageCategory> _repositoryImplementation = new RepositoryImplementation<AUsageCategory>();

        public override Dictionary<string, IProvider<AUsageCategory>> Items => _repositoryImplementation.Items;

        public override AUsageCategory GetItem(string categoryId)
        {
            return _repositoryImplementation.GetItem(categoryId);
        }

        public override T GetItem<T>(string categoryId)
        {
            return _repositoryImplementation.GetItem<T>(categoryId);
        }

        public override void RegisterItem(IProvider<AUsageCategory> category, bool addOnlyWhenMissing = true)
        {
            _repositoryImplementation.RegisterItem(category, addOnlyWhenMissing);
        }

        // deprecated
        public override ViewModel GetViewModel()
        {
            return _viewModel ??= new UsageCollectionViewModel(GameTexts.FindText("str_mission_library_usages"), this, null);
        }

        public override void OnUsageCategorySelected(AUsageCategory usageCategory)
        {
            // deprecated
        }

        public override void Clear()
        {
            _viewModel = null;
            foreach (var usageCategory in Items)
            {
                usageCategory.Value.Clear();
            }
        }
    }
}