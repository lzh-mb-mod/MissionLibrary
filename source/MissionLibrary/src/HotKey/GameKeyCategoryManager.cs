using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionLibrary.HotKey.Category;

namespace MissionLibrary.HotKey
{
    public class GameKeyCategoryManager
    {
        public Dictionary<string, IGameKeyCategory> Categories { get; set; }

        public void AddCategories(IGameKeyCategory category, bool addOnlyWhenMissing = true)
        {
            if (Categories.ContainsKey(category.GameKeyCategoryId))
            {
                if (addOnlyWhenMissing)
                    return;

                Categories[category.GameKeyCategoryId] = category;
            }

            Categories.Add(category.GameKeyCategoryId, category);

            category.Load();
            category.Save();
        }
    }
}
