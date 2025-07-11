using MissionLibrary.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionLibrary.Category
{
    public abstract class AItem<T> : ATag<T> where T: AItem<T>
    {
        public abstract string ItemId { get; }
    }
}
