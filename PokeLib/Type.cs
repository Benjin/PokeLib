using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PokeLib
{
    public class Type : APIObject
    {
        public const int NUM_TYPES = 18;
        public enum Effectiveness { NeutralAgainst, SuperEffectiveAgainst, NotVeryEffectiveAgainst, UselessAgainst,
                             NeutralTo, WeakTo, ResistantTo, ImmuneTo };

        private static Dictionary<int, Type> cachedTypes = new Dictionary<int, Type>();
        private static Dictionary<string, Type> cachedTypeNames = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private static Effectiveness[,] effectivenessMatrix = new Effectiveness[NUM_TYPES + 1, NUM_TYPES + 1]; // [attacking type, defending type]; +1 accounts for the 1-based indexing of the API
        public static Effectiveness[,] EffectivenessMatrix { get { return effectivenessMatrix; } }

        private static Dictionary<string, string> typeColors = new Dictionary<string, string>() { { "normal", "#A0A070" },
                                                                                                  { "fire", "#E77422" },
                                                                                                  { "water", "#4F7DED" },
                                                                                                  { "electric", "#F4C81A" },
                                                                                                  { "grass", "#6BBA44" },
                                                                                                  { "ice", "#8CD3D3" },
                                                                                                  { "fighting", "#B22C25" },
                                                                                                  { "poison", "#933B93" },
                                                                                                  { "ground", "#DBB54E" },
                                                                                                  { "flying", "#A68FED" },
                                                                                                  { "psychic", "#F7356F" },
                                                                                                  { "bug", "#A3B21F" },
                                                                                                  { "rock", "#A69133" },
                                                                                                  { "ghost", "#A69133" },
                                                                                                  { "dragon", "#5C1EF3" },
                                                                                                  { "dark", "#614C3E" },
                                                                                                  { "steel", "#A9A9C6" },
                                                                                                  { "fairy", "#E384E3" },
                                                                                                 }; // TODO: replace these with a real Color class

        public static void InitTypes()
        {
            Parallel.For(1, NUM_TYPES + 1 /* 1 through 18+1 for the full range of 18 types*/, i =>
            {
                Get(i); // puts all the (incomplete) types in the top-level cache
            });

            Parallel.ForEach(cachedTypes.Values, type =>
            {
                JObject data = type.GetJsonFromCache();
                addLabels(type, (JArray)data["super_effective"], Effectiveness.SuperEffectiveAgainst);
                addLabels(type, (JArray)data["ineffective"], Effectiveness.NotVeryEffectiveAgainst);
                addLabels(type, (JArray)data["no_effect"], Effectiveness.UselessAgainst);
            });
        }

        private static void addLabels(Type type, JArray otherTypes, Effectiveness level)
        {
            foreach (JToken otherTypeToken in otherTypes)
            {
                String otherName = (string)otherTypeToken["name"];
                Type otherType = cachedTypeNames[otherName];
                effectivenessMatrix[type.Id, otherType.Id] = level;
            }
        }

        public static Type Get(int i)
        {
            if (!cachedTypes.ContainsKey(i))
            {
                try
                {
                    cachedTypes[i] = new Type(i, true);
                    cachedTypeNames[cachedTypes[i].Name] = cachedTypes[i];
                }
                catch { return null; }
            }

            return cachedTypes[i];
        }

        public static Type Get(string s)
        {
            if (!cachedTypeNames.ContainsKey(s))
                InitTypes();
            return cachedTypeNames[s];
        }

        public Type(int id, bool constructNow = false)
        {
            this.id = id;

            try
            {
                if (constructNow)
                    this.Create().Wait();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override async Task Create()
        {
            JObject data = await Utilities.GetType(id);

            if (data == null)
                throw new KeyNotFoundException("Type ID #" + id + " does not exist");

            name = (string)data["name"];
            id = (int)data["id"];
            resourcePath = (string)data["resource_uri"];
        }
    }
}
