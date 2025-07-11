using MissionLibrary.Repository;
using System;

namespace MissionLibrary.HotKey
{
    public abstract class AGameKeyCategory : AItem<AGameKeyCategory>
    {
        public abstract IGameKeySequence GetGameKeySequence(int i);

        public abstract void Save();

        public abstract void Load();
        public abstract AHotKeyConfigVM CreateViewModel(Action<IHotKeySetter> onKeyBindRequest);
    }
}
