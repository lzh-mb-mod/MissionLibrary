using MissionLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.Usage
{
    public class UsageCategoryData
    {
        public UsageCategoryData(TextObject name, List<TextObject> texts)
        {
            Name = name;
            UsageList = texts;
        }

        public TextObject Name { get; }
        public List<TextObject> UsageList { get; }
    }
}