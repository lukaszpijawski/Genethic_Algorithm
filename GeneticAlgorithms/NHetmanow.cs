using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{    
    public class NHetmanow : AlgorytmGenetyczny<byte[]>
    {
        #region Prywatne pola
        private static int _liczbaHetmanow = 8;
        private const int _domyslnyRozmiarPopulacji = 100;
        private const float _domyslnePrawdopodobienstwoMutacji = 0.01f;
        private HashSet<Point<byte>>[,] tablicaSzachowanPoPrzekatnej;
        PointEqualityComparer<byte> pointEqualityComparer = new PointEqualityComparer<byte>();
        private float _maksymalnaLiczbaSzachowan;
        private List<byte> _numeryKolumn;
        private float _epsilon;
        #endregion

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
            _maksymalnaLiczbaSzachowan = (float)(_liczbaHetmanow - 1) * (float)_liczbaHetmanow / 2.0f;
            //tablicaSzachowanPoPrzekatnej = StworzTabliceSzachowanPoPrzekatnej(_liczbaHetmanow);
            _numeryKolumn = ZwrocListeNumerowKolumn(_liczbaHetmanow);
            _epsilon = 0.6f * (1.0f / _maksymalnaLiczbaSzachowan);
        }
        #endregion

        #region Koniec
        protected override byte[] Koniec(bool bestPossible = false)
        {
            if (!bestPossible && (1.0f - ObliczPrzystosowanie(najlepszyOsobnik)) > _epsilon)
            {
                return null;
            }
            return najlepszyOsobnik;
        }
        #endregion

        #region Krzyzuj
        protected override void Krzyzuj(byte[] osobnik1, byte[] osobnik2, out byte[] nowyOsobnik1, out byte[] nowyOsobnik2)
        {
            Random rand = new Random();
            int indeksPodzialu = rand.Next(0, osobnik1.Length);//ObliczIndeksPodzialu(osobnik1.Length);
            var wewnetrznyNowyOsobnik1 = new byte[osobnik1.Length];
            var wewnetrznyNowyOsobnik2 = new byte[osobnik1.Length];
     
            for (int i = 0; i <= indeksPodzialu; i++)
            {
                wewnetrznyNowyOsobnik1[i] = osobnik1[i];
                wewnetrznyNowyOsobnik2[i] = osobnik2[i];
            }

            for (int i = indeksPodzialu + 1; i < osobnik2.Length; i++)
            {
                wewnetrznyNowyOsobnik1[i] = osobnik2[i];
                wewnetrznyNowyOsobnik2[i] = osobnik1[i];
            }
                
            nowyOsobnik1 = wewnetrznyNowyOsobnik1;
            nowyOsobnik2 = wewnetrznyNowyOsobnik2;
        }

        private int ObliczIndeksPodzialu(int dlugoscOsobnika)
        {
            int indeksPodzialu = dlugoscOsobnika / 2 - 1;            
            return indeksPodzialu;
        }
        #endregion

        #region Generowanie populacji
        protected override byte[][] GenerujLosowaPopulacje(int rozmiar)
        {
            var generatorLiczbLosowych = new Random();            
            byte[][] losowaPopulacja = new byte[rozmiar][];
            for (int i = 0; i < rozmiar; i++)
            {
                losowaPopulacja[i] = new byte[_liczbaHetmanow];
                var numeryKolumn = new List<byte>(_numeryKolumn);
                for (int j = 0; j < losowaPopulacja[i].Length; j++)                
                {
                    var indeks = generatorLiczbLosowych.Next(0, numeryKolumn.Count - 1);
                    losowaPopulacja[i][j] = numeryKolumn[indeks];
                    numeryKolumn.RemoveAt(indeks);
                }
            }            

            return losowaPopulacja;
        }
        
        private List<byte> ZwrocListeNumerowKolumn(int rozmiar)
        {
            var numeryKolumn = new List<byte>(rozmiar);
            for (int i = 0; i < rozmiar; i++)
            {
                numeryKolumn.Add((byte)(i));
            }
            return numeryKolumn.ToList();
        }
        #endregion

        #region Mutuj
        protected override byte[] Mutuj(byte[] osobnik)
        {
            Random generatorLiczbLosowych = new Random();
            var numeryKolumn = _numeryKolumn;
            if (generatorLiczbLosowych.NextDouble() <= PrawdopodobienstwoMutacji)
            {
                var indeks = generatorLiczbLosowych.Next(0, osobnik.Length - 1);
                var wartosc = numeryKolumn[generatorLiczbLosowych.Next(0, numeryKolumn.Count - 1)];
                osobnik[indeks] = wartosc;
            }            
            return osobnik;
        }
        #endregion

        #region Obliczanie przystosowania
        protected override float ObliczPrzystosowanie(byte[] osobnik)
        {
            var liczbaSzachowan = ObliczLiczbeSzachowan(osobnik);
            if (liczbaSzachowan == 0)
            {
                return 1.0f;
            }
            return ((float)_maksymalnaLiczbaSzachowan - (float)liczbaSzachowan) / (float)_maksymalnaLiczbaSzachowan;
        }
        #endregion

        #region Obliczanie liczby szachowań
        private int ObliczLiczbeSzachowan(byte[] osobnik)
        {
            int liczbaSzachowan = 0;
            byte[] tymczasowyOsobnik = osobnik.ToArray();
            var osobnikJakoHashSet = new HashSet<Point<byte>>();
            for (byte i = 0; i < osobnik.Length; i++)
            {
                var pole = new Point<byte> { X = i, Y = osobnik[i] };
                osobnikJakoHashSet.Add(pole);
            }

            liczbaSzachowan += ObliczLiczbeSzachowanPoPrzekatnej(osobnikJakoHashSet);
            liczbaSzachowan += ObliczLiczbeSzachowanPoKolumnie(osobnik);

            return liczbaSzachowan;
        }

        private int ObliczLiczbeSzachowanPoPrzekatnej(HashSet<Point<byte>> osobnikJakoHashSet)
        {
            int liczbaSzachowan = 0;
            foreach (var hetman in osobnikJakoHashSet)
            {
                foreach (var hetman2 in osobnikJakoHashSet)
                {
                    liczbaSzachowan += CzySaNaPrzekatnej(hetman, hetman2) ? 1 : 0;
                }
                //liczbaSzachowan += tablicaSzachowanPoPrzekatnej[hetman.X, hetman.Y].Where(a => osobnikJakoHashSet.Contains(a, pointEqualityComparer)).Count() - 1;
            }

            return liczbaSzachowan / 2;
        }

        private int ObliczLiczbeSzachowanPoKolumnie(byte[] osobnik)
        {
            int liczbaSzachowan = 0;
            foreach (var hetman in osobnik)
            {
                liczbaSzachowan += osobnik.Where(a => a == hetman).Count() - 1;
            }

            return liczbaSzachowan / 2;
        }


        private HashSet<Point<byte>>[,] StworzTabliceSzachowanPoPrzekatnej(int rozmiar)
        {
            var tablicaSzachowan = new HashSet<Point<byte>>[rozmiar, rozmiar];
            for (int i = 0; i < tablicaSzachowan.GetLength(0); i++)
            {
                for (int j = 0; j < tablicaSzachowan.GetLength(1); j++)
                {
                    var lista = new HashSet<Point<byte>>();

                    for (int k = 0; k < rozmiar; k++)
                    {
                        var y = j - (i - k);
                        if (y >= 0 && y < rozmiar)
                        {
                            lista.Add(new Point<byte> { X = (byte)k, Y = (byte)y });
                        }
                        y = j + (i - k);
                        if (y >= 0 && y < rozmiar)
                        {
                            lista.Add(new Point<byte> { X = (byte)k, Y = (byte)y });
                        }
                    }                    
                    tablicaSzachowan[i, j] = new HashSet<Point<byte>>(lista.GroupBy(a => new { a.X, a.Y }).Select(a => a.Last()));
                }
            }

            return tablicaSzachowan;
        }

        private bool CzySaNaPrzekatnej(Point<byte> liczba1, Point<byte> liczba2)
        {
            return Math.Abs(liczba1.X - liczba2.X) == Math.Abs(liczba1.Y - liczba2.Y);
        }

        private bool CzySaWTejSamejLinii(Point<byte> liczba1, Point<byte> liczba2)
        {
            return (liczba1.Y == liczba2.Y) || (liczba1.X == liczba2.X);
        }

        private bool CzySaWTejSamejLinii(byte liczba1, byte liczba2)
        {
            return liczba1 == liczba2;
        }
        #endregion

    }
}
