using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Diagnostics;

namespace PokeLib
{
    public class Utilities
    {
        private static HttpClient fetcher = new HttpClient();
        private static string baseUri = "http://pokeapi.co";
        private static string prefix = "/api/v1/";

        public static async Task<JObject> GetPokemon(int n)
        {
            return await GetData(prefix + "pokemon/" + n + "/");
        }

        public static async Task<JObject> GetType(int n)
        {
            return await GetData(prefix + "type/" + n + "/");
        }

        private static Dictionary<string, JObject> resourceCache = new Dictionary<string, JObject>(); // TODO: Stale check

        public static async Task<JObject> GetData(string resourcePath, bool forceRefresh = false)
        {
            if (!resourceCache.ContainsKey(resourcePath) || forceRefresh)
                resourceCache[resourcePath] = await FetchDataFromServer(resourcePath);

            return resourceCache[resourcePath];
        }

        public static async Task<JObject> FetchDataFromServer(string resourcePath)
        {
            try
            {
                // TODO: there may be an error message in the form of JSON returned; that'll break things

                string json = await fetcher.GetStringAsync(baseUri + resourcePath);
                return JObject.Parse(json);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
        }

        public static bool TryGetJsonFromCache(string resourcePath, out JObject value)
        {
            if (resourceCache.ContainsKey(resourcePath))
            {
                value = resourceCache[resourcePath];
                return true;
               
            }
            else
            {
                value = null;
                return false;
            }
        }
    }
}
