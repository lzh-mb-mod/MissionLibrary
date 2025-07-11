using MissionSharedLibrary.View.ViewModelCollection.Basic;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
    public class UsageCategoryViewModel : ViewModel
    {
        public UsageCategoryViewModel(TextObject title, List<TextObject> texts)
        {
            Title = title;
            foreach (var text in texts)
            {
                Texts.Add(new TextViewModel(text));
            }
        }

        [DataSourceProperty]
        public TextObject Title { get; }

        [DataSourceProperty]
        public MBBindingList<TextViewModel> Texts { get; }
    }
}