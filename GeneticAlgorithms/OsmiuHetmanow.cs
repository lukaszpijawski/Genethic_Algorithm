using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    public class OsmiuHetmanow : AlgorytmGenetyczny<byte[]>
    {
        protected override byte[] Koniec(bool bestPossible = false)
        {
            throw new NotImplementedException();
        }

        protected override void Krzyzuj(byte[] osobnik1, byte[] osobnik2, out byte[] nowyOsobnik1, out byte[] nowyOsobnik2)
        {
            throw new NotImplementedException();
        }

        protected override byte[][] LosowaPopulacja(int rozmiar)
        {
            throw new NotImplementedException();
        }

        protected override byte[] Mutacja(byte[] osobnik)
        {
            throw new NotImplementedException();
        }

        protected override float Przystosowanie(byte[] osobnik)
        {
            throw new NotImplementedException();
        }
    }
}
