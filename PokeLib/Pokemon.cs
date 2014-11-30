using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace PokeLib
{
    public class Pokemon : APIObject
    {

        private static Dictionary<int, Pokemon> cachedPokemon = new Dictionary<int, Pokemon>();

        private Stats stats;

        public int HP { get { return stats.HP; } }
        public int Attack { get { return stats.Attack; } }
        public int Defense { get { return stats.Defense; } }
        public int SpecialAttack { get { return stats.SpecialAttack; } }
        public int SpecialDefense { get { return stats.SpecialDefense; } }
        public int Speed { get { return stats.Speed; } }

        private List<PokeType> types;
        public List<PokeType> Types { get { return types; } }

        public PokeType Type1 { get { return types[0]; } }
        public PokeType Type2 { get { return types[1]; } }

        private Byte[] image;
        public Byte[] Image { get { return image; } }

        private Pokemon(int id)
        {
            this.id = id;
            types = new List<PokeType>();
        }

        private async Task<Pokemon> InitializeAsync()
        {
            Task<byte[]> imageTask = Utilities.GetPokemonImage(id);
            JObject data = await Utilities.GetPokemon(id);

            if (data == null)
                throw new KeyNotFoundException("Pokemon ID #" + id + " does not exist");

            

            name = (string)data["name"];
            resourcePath = (string)data["resource_uri"];
            stats = new Stats(data);
            foreach (JToken type in (JArray)data["types"])
            {
                types.Add(await PokeType.Get((string)type["name"]));
            }

            image = await imageTask;

            return this;
        }

        public static async Task<Pokemon> Get(int id)
        {
            Pokemon result = new Pokemon(id);

            if (!cachedPokemon.ContainsKey(id))
            {
                cachedPokemon[id] = await result.InitializeAsync();
            }

            return cachedPokemon[id];
        }

        public override string ToString()
        {
            string result = String.Format("{0}. {1}", id, name);
            return result;
        }
    }
}