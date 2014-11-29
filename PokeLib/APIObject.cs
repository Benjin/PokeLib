using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace PokeLib
{
    public abstract class APIObject
    {
        internal int id;
        public int Id { get { return id; } }

        internal string name;
        public string Name { get { return name; } }

        internal string resourcePath;
        public string ResourcePath { get { return resourcePath; } }

        public abstract Task Create();

        public JObject GetJsonFromCache()
        {
            JObject result;
            if(Utilities.TryGetJsonFromCache(ResourcePath, out result))
                return result;
            else
            {
                Debug.WriteLine("Error: Json not found in cache for instantiated object");
                return null;
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
