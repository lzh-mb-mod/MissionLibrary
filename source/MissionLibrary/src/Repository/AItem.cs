using MissionLibrary.Provider;

namespace MissionLibrary.Repository
{
    public abstract class AItem<T> : ATag<T> where T: AItem<T>
    {
        public abstract string ItemId { get; }
    }
}
