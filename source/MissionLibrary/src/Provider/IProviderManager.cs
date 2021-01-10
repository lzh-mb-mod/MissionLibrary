using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionLibrary.Provider
{
    public interface IProviderManager
    {
        void RegisterProvider<T>(IVersionProvider<T> newProvider) where T : ATag<T>;
    }
}
