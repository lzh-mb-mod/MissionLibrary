using System.Collections.Generic;

namespace MissionLibrary.HotKey
{
    public class GameKeyCategoryManager
    {
        public Dictionary<string, IGameKeyCategory> Categories { get; set; } = new Dictionary<string, IGameKeyCategory>();

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
