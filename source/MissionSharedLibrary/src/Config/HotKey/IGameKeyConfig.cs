using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionSharedLibrary.Config.HotKey
{
    public interface IGameKeyConfig
    {
        SerializedGameKeyCategory Category { get; set; }
        bool Serialize();
        bool Deserialize();
    }
}
