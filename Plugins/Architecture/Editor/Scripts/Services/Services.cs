using System;
using System.Collections.Generic;

namespace Ludwell.Scene.Editor
{
    public class Services
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Clear()
        {
            _services.Clear();
        }
        
        public static void Add<T>(object service)
        {
            var type = typeof(T);
            _services.TryAdd(type, service);
        }
        
        public static void Remove<T>()
        { 
            var type = typeof(T);
            if (!_services.ContainsKey(type)) return;
            _services.Remove(type);
        }

        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service)) return default; 
            return service as T;
        }
        
        public static bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var fetchedService))
            {
                service = null;
                return false;
            }

            service = fetchedService as T;
            return true;
        }
    }
}
