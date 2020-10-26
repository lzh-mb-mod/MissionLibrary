using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionLibrary.HotKey.Category;

namespace MissionLibrary.HotKey
{
    public static class HotKeyCategoryManager
    {
        public static Dictionary<string, IGameKeyCategory> Categories { get; set; }

        public static void AddDefaultCategories()
        {
            AddCategories(new MissionLibraryGeneralHotKeyCategory());
        }

        public static void AddCategories(IGameKeyCategory category, bool addOnlyWhenMissing = true)
        {
            if (Categories.ContainsKey(category.GameKeyCategoryId))
            {
                if (addOnlyWhenMissing)
                    return;

                Categories[category.GameKeyCategoryId] = category;
            }

            Categories.Add(category.GameKeyCategoryId, category);
        }
    }
}
