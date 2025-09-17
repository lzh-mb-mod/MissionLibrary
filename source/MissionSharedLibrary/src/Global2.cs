using MissionLibrary;
using MissionLibrary.Provider;
using System.Collections.Generic;

namespace MissionSharedLibrary
{
    public static class Global2
    {
        public static void Initialize()
        {
            Global.Initialize();
        }

        public static void ThirdInitialize()
        {
            Global.ThirdInitialize();
        }

        public static void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>
        {
            Global.RegisterInstance(newProvider, key);
        }

        public static T GetInstance<T>(string key = "") where T : ATag<T>
        {
            return Global.GetInstance<T>(key);
        }

        public static IEnumerable<T> GetInstances<T>() where T : ATag<T>
        {
            return Global.GetInstances<T>();
        }
        public static void Clear()
        {
            Global.Clear();
        }
    }
}
