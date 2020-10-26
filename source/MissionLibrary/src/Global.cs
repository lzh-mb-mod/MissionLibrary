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
            MissionLibraryGameKeyCategory.RegisterGameKeyCategory();
        }

        public static void Clear()
        {
            _isInitialized = false;
            GameKeyCategoryManager = null;
            MissionLibraryGameKeyCategory.Clear();
        }
    }
}
