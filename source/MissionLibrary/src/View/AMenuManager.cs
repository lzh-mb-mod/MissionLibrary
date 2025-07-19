using MissionLibrary.Provider;
using System;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionLibrary.View
{
    public abstract class AMenuManager : ATag<AMenuManager>
    {
        public static AMenuManager Get()
        {
            return Global.GetInstance<AMenuManager>();
        }

        public event Action OnMenuClosedEvent;

        public void OnMenuClosed()
        {
            OnMenuClosedEvent?.Invoke();
        }
        
        public abstract AMenuClassCollection MenuClassCollection { get; }

        // deprecated
        public abstract MissionView CreateMenuView();

        // deprecated
        public abstract MissionView CreateGameKeyConfigView();
        public abstract void RequestToOpenMenu();
        public abstract void RequestToCloseMenu();

        public abstract void RequestToOpenUsageView();
    }
}
