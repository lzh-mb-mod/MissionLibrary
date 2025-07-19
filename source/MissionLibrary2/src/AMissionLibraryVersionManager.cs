using MissionLibrary;
using MissionLibrary.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionLibrary2
{
    public abstract class AMissionLibraryVersionManager: ATag<AMissionLibraryVersionManager>
    {
        public static AMissionLibraryVersionManager Get()
        {
            return Global.GetInstance<AMissionLibraryVersionManager>();
        }

        public abstract void RegisterInstances();
    }
}
