using MissionLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using System;
using TaleWorlds.Library;

namespace MissionSharedLibrary.Usage
{
    public class UsageCategory : AUsageCategory
    {
        private readonly UsageCategoryViewModel _viewModel;
        public override string ItemId { get; }

        public override ViewModel ViewModel => _viewModel;

        public UsageCategory(string itemId, UsageCategoryData usageCategoryData)
        {
            ItemId = itemId;
            _viewModel = new UsageCategoryViewModel(usageCategoryData, OnSelect);
        }

        public override void UpdateSelection(bool isSelected)
        {
            _viewModel.IsSelected = isSelected;
        }

        private void OnSelect()
        {
            AUsageCategoryManager.Get()?.OnUsageCategorySelected(this);
        }
    }
}