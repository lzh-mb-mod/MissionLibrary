using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionLibrary.Provider
{
    public abstract class ATag<T> where T : ATag<T>
    {
        public virtual T Self => (T)this;
    }
}
