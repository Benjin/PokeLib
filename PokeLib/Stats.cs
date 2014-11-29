using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PokeLib
{
    class Stats
    {
        public int HP, Attack, Defense, SpecialAttack, SpecialDefense, Speed;

        public Stats(JObject data)
        {
            HP = (int)data["hp"];
            Attack = (int)data["attack"];
            Defense = (int)data["defense"];
            SpecialAttack = (int)data["sp_atk"];
            SpecialDefense = (int)data["sp_def"];
            Speed = (int)data["speed"];
        }
    }
}
