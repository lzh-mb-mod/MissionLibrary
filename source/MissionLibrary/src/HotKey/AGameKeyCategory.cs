using MissionLibrary.Category;
using System;

namespace MissionLibrary.HotKey
{
    public abstract class AGameKeyCategory : ACategory<AGameKeyCategory>
    {
        public abstract IGameKeySequence GetGameKeySequence(int i);

        public abstract void Save();

        public abstract void Load();
        public abstract AHotKeyConfigVM CreateViewModel(Action<IHotKeySetter> onKeyBindRequest);
    }
}
