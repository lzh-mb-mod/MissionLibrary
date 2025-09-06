namespace MissionSharedLibrary.Config.HotKey
{
    public abstract class GameKeyConfigBase<T> : MissionConfigBase<T>, IGameKeyConfig where T: GameKeyConfigBase<T>
    {
        public SerializedGameKeyCategory Category { get; set; } = new SerializedGameKeyCategory();

        protected override void CopyFrom(T other)
        {
            Category = other.Category;
        }
    }
}
