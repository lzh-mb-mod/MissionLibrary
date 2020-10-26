using MissionLibrary.HotKey;
using MissionLibrary.HotKey.Category;

namespace MissionLibrary
{
    public static class Global
    {
        private static bool _isInitialized;

        public static GameKeyCategoryManager GameKeyCategoryManager { get; set; }

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            GameKeyCategoryManager = new GameKeyCategoryManager();
            MissionLibraryGameKeyCategories.RegisterGameKeyCategory();
        }

        public static void Clear()
        {
            _isInitialized = false;
            GameKeyCategoryManager = null;
            MissionLibraryGameKeyCategories.Clear();
        }
    }
}
