using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    public class NHetmanow : AlgorytmGenetyczny<byte[]>
    {
        private static int _liczbaHetmanow = 8;
        private const int _domyslnyRozmiarPopulacji = 10;
        private const float _domyslnePrawdopodobienstwoMutacji = 0.2f;

        #region Konstruktory
        public NHetmanow() : this(_liczbaHetmanow, _domyslnyRozmiarPopulacji, _domyslnePrawdopodobienstwoMutacji)
        {
        }

        public NHetmanow(int liczbaHetmanow) : this(liczbaHetmanow, _domyslnyRozmiarPopulacji, _domyslnePrawdopodobienstwoMutacji)
        {
        }

        public NHetmanow(int liczbaHetmanow, int rozmiarPopulacji) : this(liczbaHetmanow, rozmiarPopulacji, _domyslnePrawdopodobienstwoMutacji)
        {
        }

        public NHetmanow(int liczbaHetmanow, float prawdopodobienstwoMutacji) : this(liczbaHetmanow, _domyslnyRozmiarPopulacji, prawdopodobienstwoMutacji)
        {
        }

        public NHetmanow(int liczbaHetmanow, int rozmiarPopulacji, float prawdopodobienstwoMutacji)
        {
            _liczbaHetmanow = liczbaHetmanow;
            RozmiarPopulacji = rozmiarPopulacji;
            PrawdopodobienstwoMutacji = prawdopodobienstwoMutacji;
        }
        #endregion

        protected override byte[] Koniec(bool bestPossible = false)
        {
            throw new NotImplementedException();
        }

        protected override void Krzyzuj(byte[] osobnik1, byte[] osobnik2, out byte[] nowyOsobnik1, out byte[] nowyOsobnik2)
        {
            throw new NotImplementedException();
        }

        #region Generowanie populacji
        protected override byte[][] GenerujLosowaPopulacje(int rozmiar)
        {
            Random generatorLiczbLosowych = new Random();
            List<byte> numeryKolumn = ZwrocTabliceNumerowKolumn(_liczbaHetmanow);
            byte[][] losowaPopulacja = new byte[rozmiar][];
            for (int i = 0; i < rozmiar; i++)
            {
                losowaPopulacja[i] = new byte[_liczbaHetmanow];
                for (int j = 0; j < losowaPopulacja[i].Length; j++)
                {
                    var indeks = generatorLiczbLosowych.Next(0, numeryKolumn.Count - 1);
                    losowaPopulacja[i][j] = numeryKolumn[indeks];
                    numeryKolumn.RemoveAt(indeks);
                }
            }
            return losowaPopulacja;
        }
        
        private List<byte> ZwrocTabliceNumerowKolumn(int rozmiar)
        {
            List<byte> numeryKolumn = new List<byte>(rozmiar);
            for (int i = 0; i < rozmiar; i++)
            {
                numeryKolumn[i] = (byte)(i + 1);
            }
            return numeryKolumn;
        }
        #endregion

        #region Mutuj
        protected override byte[] Mutuj(byte[] osobnik)
        {
            Random generatorLiczbLosowych = new Random();
            List<byte> numeryKolumn = ZwrocTabliceNumerowKolumn(_liczbaHetmanow);
            var indeks = generatorLiczbLosowych.Next(0, osobnik.Length - 1);
            var wartosc = numeryKolumn[generatorLiczbLosowych.Next(0, numeryKolumn.Count - 1)];
            osobnik[indeks] = wartosc;
            return osobnik;
        }
        #endregion

        protected override float ObliczPrzystosowanie(byte[] osobnik)
        {
            throw new NotImplementedException();
        }
    }
}
