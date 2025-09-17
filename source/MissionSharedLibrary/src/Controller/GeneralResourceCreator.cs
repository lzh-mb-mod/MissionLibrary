using MissionLibrary;
using MissionLibrary.Controller;
using MissionSharedLibrary.HotKey;

namespace MissionSharedLibrary.Controller
{
    public class GeneralResourceCreator : AResourceCreator
    {
        public GeneralResourceCreator()
        {
            Global.GetInstance<AMissionStartingManager>().AddHandler(new DefaultMissionStartingHandler());
            GeneralGameKeyCategory.RegisterGameKeyCategory();
        }
    }
}
