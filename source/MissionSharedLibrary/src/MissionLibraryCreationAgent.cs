using MissionLibrary;
using MissionLibrary.Provider;
using MissionLibrary2;
using MissionSharedLibrary.Controller;
using MissionSharedLibrary.Controller.Camera;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.View;
using System;
namespace MissionSharedLibrary
{
    public class MissionLibraryCreationAgent : AMissionLibraryCreationAgent
    {
        private bool _registered = false;

        public override void RegisterInstances()
        {
            if (_registered)
                return;
            _registered = true;
            RegisterProvider(() => new GameKeyCategoryManager(), new Version(2, 0));
            RegisterProvider(() => new CameraControllerManager(), new Version(2, 0));
            RegisterProvider(() => new MissionStartingManager(), new Version(2, 0));
            RegisterProvider(() => new MissionLibraryMissionLogicFactory(), new Version(2, 0));
            RegisterProvider(() => new DefaultMissionStartingHandlerAdder(), new Version(2, 0));
            RegisterProvider(() => new MenuManager(), new Version(2, 0));
        }
        private static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
        {
            Global.RegisterProvider(VersionProviderCreator.Create(creator, providerVersion), key);
        }
    }

}