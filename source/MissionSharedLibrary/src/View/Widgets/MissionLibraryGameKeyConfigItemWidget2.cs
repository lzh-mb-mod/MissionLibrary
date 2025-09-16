using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace MissionLibrary.View.Widgets
{
    public class MissionLibraryGameKeyConfigItemWidget2 : ListPanel
    {
        private MissionLibraryGameKeyConfigWidget2 _screenWidget;
        private bool _initialized;

        public MissionLibraryGameKeyConfigItemWidget2(UIContext context)
          : base(context)
        {
        }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);
            if (!_initialized)
            {
                _initialized = true;
                if (V1ui != null)
                {
                    this.V1ui.IsVisible = VMVersion == null || VMVersion == "v1";
                }
                if (V2ui != null)
                {
                    this.V2ui.IsVisible = VMVersion == "v2";
                }
            }
            if (_screenWidget == null)
                _screenWidget = EventManager.Root.GetChild(0).FindChild("Options", true) as MissionLibraryGameKeyConfigWidget2;

        }

        protected override void OnChildAdded(Widget child)
        {
            base.OnChildAdded(child);
            child.boolPropertyChanged += Child_BoolPropertyChanged;
            child.EventFire += OnEventFired;
        }

        public override void OnBeforeRemovedChild(Widget widget)
        {
            base.OnBeforeRemovedChild(widget);
            widget.boolPropertyChanged -= Child_BoolPropertyChanged;
            widget.EventFire -= OnEventFired;
        }

        protected override void OnHoverBegin()
        {
            base.OnHoverBegin();
            SetCurrentOption(false, false);
        }

        protected override void OnHoverEnd()
        {
            base.OnHoverEnd();
            ResetCurrentOption();
        }

        private void SetCurrentOption(
          bool fromHoverOverDropdown,
          bool fromBooleanSelection,
          int hoverDropdownItemIndex = -1)
        {
            _screenWidget?.SetCurrentOption(this, null);
        }

        private void ResetCurrentOption() => _screenWidget?.SetCurrentOption(null, null);

        private void RegisterHoverEvents()
        {
            foreach (Widget allChild in GetAllChildrenRecursive())
                allChild.boolPropertyChanged += Child_BoolPropertyChanged;
        }

        private void Child_BoolPropertyChanged(
          PropertyOwnerObject childWidget,
          string propertyName,
          bool propertyValue)
        {
            if (propertyName != "IsHovered")
                return;
            if ((bool)propertyValue)
                SetCurrentOption(false, false);
            else
                ResetCurrentOption();
        }

        private void OnEventFired(Widget w, string name, object[]obj)
        {
            if (name == "ItemAdd")
            {
                var child = obj.FirstOrDefault() as Widget;
                if (child != null)
                {
                    child.boolPropertyChanged += Child_BoolPropertyChanged;
                    child.EventFire += OnEventFired;
                }
            }
            else if (name == "ItemRemove")
            {
                var child = obj.FirstOrDefault() as Widget;
                if (child != null)
                {
                    child.boolPropertyChanged -= Child_BoolPropertyChanged;
                    child.EventFire -= OnEventFired;
                }
            }
        }

        public string OptionTitle { get; set; }

        public string OptionDescription { get; set; }

        public string VMVersion { get; set; }

        public Widget V2ui { get; set; }
        
        public Widget V1ui { get; set; }
    }
}
