using System;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
    public abstract class MissionMenuVMBase : ViewModel
    {
        private readonly Action _closeMenu;

        public virtual void CloseMenu()
        {
            _closeMenu?.Invoke();
        }

        protected MissionMenuVMBase(Action closeMenu)
        {
            _closeMenu = closeMenu;
        }
    }
}
