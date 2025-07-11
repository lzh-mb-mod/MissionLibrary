using MissionLibrary.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionLibrary.Category
{
    public abstract class ACategory<T> : ATag<T> where T: ACategory<T>
    {
        public abstract string CategoryId { get; }
    }
}
