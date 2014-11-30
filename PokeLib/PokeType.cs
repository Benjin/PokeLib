using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PokeLib
{
    public class PokeType : APIObject
    {
        public const int NUM_TYPES = 18;
        public enum Effectiveness
        {
            NeutralAgainst, SuperEffectiveAgainst, NotVeryEffectiveAgainst, UselessAgainst,
            NeutralTo, WeakTo, ResistantTo, ImmuneTo
        };

        private static Dictionary<int, PokeType> cachedTypes = new Dictionary<int, PokeType>();
        private static Dictionary<string, PokeType> cachedTypeNames = new Dictionary<string, PokeType>(StringComparer.OrdinalIgnoreCase);

        private static Effectiveness[,] effectivenessMatrix = new Effectiveness[NUM_TYPES + 1, NUM_TYPES + 1]; // [attacking type, defending type]; +1 accounts for the 1-based indexing of the API
        public static Effectiveness[,] EffectivenessMatrix { get { return effectivenessMatrix; } }

        private static Dictionary<string, string> typeColors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "normal", "#A0A070" },
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
            { "ghost", "#624D85" },
            { "dragon", "#5C1EF3" },
            { "dark", "#614C3E" },
            { "steel", "#A9A9C6" },
            { "fairy", "#E384E3" },
            }; // TODO: replace these with a real Color class


        private static bool initialized = false;

        public static async Task InitTypes()
        {
            if (initialized) return;

            await Task.WhenAll(Enumerable.Range(1, NUM_TYPES).Select(i =>

                Get(i)
            ));

            foreach (PokeType type in cachedTypes.Values)
            {
                JObject data = type.GetJsonFromCache();
                addLabels(type, (JArray)data["super_effective"], Effectiveness.SuperEffectiveAgainst);
                addLabels(type, (JArray)data["ineffective"], Effectiveness.NotVeryEffectiveAgainst);
                addLabels(type, (JArray)data["no_effect"], Effectiveness.UselessAgainst);
            }

            initialized = true;
        }

        private static void addLabels(PokeType type, JArray otherTypes, Effectiveness level)
        {
            foreach (JToken otherTypeToken in otherTypes)
            {
                String otherName = (string)otherTypeToken["name"];
                PokeType otherType = cachedTypeNames[otherName];
                effectivenessMatrix[type.Id, otherType.Id] = level;
            }
        }

        private async Task<PokeType> InitializeAsync()
        {
            JObject data = await Utilities.GetType(id);

            if (data == null)
                throw new KeyNotFoundException("PokeType ID #" + id + " does not exist");

            name = (string)data["name"];
            id = (int)data["id"];
            resourcePath = (string)data["resource_uri"];

            return this;
        }

        public static async Task<PokeType> Get(int id)
        {
            PokeType result = new PokeType(id);

            if (!cachedTypes.ContainsKey(id))
            {
                cachedTypes[id] = await result.InitializeAsync();
                cachedTypeNames[cachedTypes[id].Name] = cachedTypes[id];
            }

            return cachedTypes[id];
        }

        public static async Task<PokeType> Get(string name)
        {
            if (!cachedTypeNames.ContainsKey(name))
                await InitTypes();
            return cachedTypeNames[name];
        }

        private PokeType(int id)
        {
            this.id = id;
        }

        public string Color
        {
            get
            {
                return typeColors[Name];
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
