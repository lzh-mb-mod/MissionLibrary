using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace MissionSharedLibrary.View.Widgets
{
    public class MissionLibraryGameKeyGroupWidget : ListPanel
    {

        public MissionLibraryGameKeyGroupWidget(UIContext context)
          : base(context)
        {
        }

        protected override void OnLateUpdate(float dt)
        {
            base.OnLateUpdate(dt);
        }




        public string OptionTitle { get; set; }

        public string OptionDescription { get; set; }
    }
}
