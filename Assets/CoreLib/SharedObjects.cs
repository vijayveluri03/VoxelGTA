using System.Collections;
using System.Collections.Generic;

using System.Collections.Generic;

namespace Core
{
    public class SharedObjects<T> where T : class
    {
        public void Add(string name, T obj)
        {
            Core.QLogger.Assert(!objects.ContainsKey(name));
            Core.QLogger.Assert(obj != null);
            objects[name] = obj;
        }

        public void Set(string name, T obj)
        {
            //Core.QLogger.Assert(!objects.ContainsKey(name));
            Core.QLogger.Assert(obj != null);
            objects[name] = obj;
        }

        public U TryFetch<U>(string name) where U : class
        {
            if (objects.ContainsKey(name))
            {
                Core.QLogger.Assert(objects[name] is U);
                return objects[name] as U;
            }
            return null;
        }

        public T TryFetch(string name)
        {
            if (objects.ContainsKey(name))
            {
                return objects[name];
            }
            return null;
        }

        public T Fetch(string name)
        {
            Core.QLogger.Assert(objects.ContainsKey(name));
            return objects[name];
        }

        public U Fetch<U>(string name) where U : class
        {
            Core.QLogger.Assert(objects.ContainsKey(name));
            return objects[name] as U;
        }


        private Dictionary<string, T> objects = new Dictionary<string, T>();
    }
}