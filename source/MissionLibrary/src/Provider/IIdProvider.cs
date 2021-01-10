using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionLibrary.Provider
{
    public interface IIdProvider
    {
        string Id { get; }
        void ForceCreate();
        void Clear();
    }

    public interface IIdProvider<out T> : IIdProvider where T : ATag<T>
    {
        T Value { get; }
    }
}
