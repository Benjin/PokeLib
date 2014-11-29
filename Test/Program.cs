using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokeLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestPokemon();
            //TestTypes();

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
        }

        static void TestTypes()
        {
            PokeLib.Type.InitTypes();
            var table = PokeLib.Type.EffectivenessMatrix;

            StringBuilder sb = new StringBuilder("          ");

            for (int attacking = 1; attacking <= PokeLib.Type.NUM_TYPES; attacking++)
            {
                sb.Append(PokeLib.Type.Get(attacking).ToString().Substring(0, 1));
            }

            Console.WriteLine(sb.ToString());

            for (int attacking = 1; attacking <= PokeLib.Type.NUM_TYPES; attacking++)
            {
                sb = new StringBuilder(PokeLib.Type.Get(attacking).ToString().PadLeft(10, ' '));

                for (int defending = 1; defending <= PokeLib.Type.NUM_TYPES; defending++)
                {
                    switch (table[attacking, defending])
                    {
                        case PokeLib.Type.Effectiveness.SuperEffectiveAgainst:
                            sb.Append("+");
                            break;
                        case PokeLib.Type.Effectiveness.NotVeryEffectiveAgainst:
                            sb.Append("-");
                            break;
                        case PokeLib.Type.Effectiveness.UselessAgainst:
                            sb.Append("x");
                            break;
                        default:
                            sb.Append(" ");
                            break;
                    }
                }

                Console.WriteLine(sb.ToString());
            }

        }

        static void TestPokemon()
        {
            PokeLib.Type.InitTypes();

            Parallel.For(1, 10, n =>
            {
                Pokemon.Get(n);
            });

            for (int i = 0; i <= 10; i++)
            {
                Pokemon p = Pokemon.Get(i);
                Console.WriteLine(p == null ? "null" : p.ToString());
            }
        }
    }
}
