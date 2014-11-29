using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PokeLib
{
    public class Pokemon : APIObject
    {
        private static Dictionary<int, Pokemon> cachedPokemon = new Dictionary<int, Pokemon>();

        public static Pokemon Get(int i)
        {
            if (!cachedPokemon.ContainsKey(i))
            {
                try { cachedPokemon[i] = new Pokemon(i, true); }
                catch { return null; }
            }

            return cachedPokemon[i];
        }

        private Stats stats;

        public int HP { get { return stats.HP; } }
        public int Attack { get { return stats.Attack; } }
        public int Defense { get { return stats.Defense; } }
        public int SpecialAttack { get { return stats.SpecialAttack; } }
        public int SpecialDefense { get { return stats.SpecialDefense; } }
        public int Speed { get { return stats.Speed; } }

        private List<Type> types;
        public List<Type> Types { get { return types; } }

        public Pokemon(int id, bool constructNow = false)
        {
            this.id = id;
            types = new List<Type>();

            try
            {
                if (constructNow)
                    this.Create().Wait();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public override async Task Create()
        {
            JObject data = await Utilities.GetPokemon(id);

            if (data == null)
                throw new KeyNotFoundException("Pokemon ID #" + id + " does not exist");

            name = (string)data["name"];
            resourcePath = (string)data["resource_uri"];
            stats = new Stats(data);
            foreach (JToken type in (JArray)data["types"])
            {
                types.Add(Type.Get((string)type["name"]));
            }
        }

        public override string ToString()
        {
            return String.Format("{0}. {1} ({2}{3})", id, name, Types[0].ToString(), Types.Count > 1 ? ", " + Types[1].ToString() : String.Empty);
        }
    }
}
