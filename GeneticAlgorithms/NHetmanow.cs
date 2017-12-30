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
        #region private fields
        private static int _liczbaHetmanow = 8;
        private const int _domyslnyRozmiarPopulacji = 10;
        private const float _domyslnePrawdopodobienstwoMutacji = 0.01f;
        private List<Point<byte>>[,] tablicaSzachowanPoPrzekatnej;
        PointEqualityComparer<byte> pointEqualityComparer = new PointEqualityComparer<byte>();
        #endregion

        #region Konstruktory
        public NHetmanow() : this(8, _domyslnyRozmiarPopulacji, _domyslnePrawdopodobienstwoMutacji)
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
            tablicaSzachowanPoPrzekatnej = StworzTabliceSzachowanPoPrzekatnej(_liczbaHetmanow);
        }
        #endregion

        protected override byte[] Koniec(bool bestPossible = false)
        {
            if (!bestPossible && Math.Abs(ObliczPrzystosowanie(najlepszyOsobnik) - 1.0f) > 0.05)
            {
                return null;
            }
            return najlepszyOsobnik;
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
            var numeryKolumn = new List<byte>(rozmiar);
            for (int i = 0; i < rozmiar; i++)
            {
                numeryKolumn.Add((byte)(i + 1));
            }
            return numeryKolumn.ToList();
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
            var liczbaSzachowan = ObliczLiczbeSzachowan(osobnik);
            return 1.0f;
        }

        private int ObliczLiczbeSzachowan(byte[] osobnik)
        {
            int liczbaSzachowan = 0;
            byte[] tymczasowyOsobnik = osobnik.ToArray();
            List<Point<byte>> osobnikJakoLista = new List<Point<byte>>(osobnik.Length);
            for (byte i = 0; i < osobnik.Length; i++)
            {
                var pole = new Point<byte> { X = i, Y = osobnik[i] };
                osobnikJakoLista.Add(pole);
            }

            liczbaSzachowan += ObliczLiczbeSzachowanPoPrzekatnej(osobnikJakoLista);
            liczbaSzachowan += ObliczLiczbeSzachowanPoKolumnie(osobnik);

            return liczbaSzachowan;
        }

        private int ObliczLiczbeSzachowanPoPrzekatnej(List<Point<byte>> osobnikJakoLista)
        {
            int liczbaSzachowan = 0;
            foreach (var hetman in osobnikJakoLista)
            {
                liczbaSzachowan += tablicaSzachowanPoPrzekatnej[hetman.X, hetman.Y].Where(a => osobnikJakoLista.Contains(a, pointEqualityComparer)).Count() - 1;
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

        private List<Point<byte>>[,] StworzTabliceSzachowanPoPrzekatnej(int rozmiar)
        {
            var rozmiarTablicy = rozmiar * rozmiar;
            var tablicaSzachowan = new List<Point<byte>>[rozmiar, rozmiar];
            for (int i = 0; i < tablicaSzachowan.GetLength(0); i++)
            {
                for (int j = 0; j < tablicaSzachowan.GetLength(1); j++)
                {
                    var lista = new List<Point<byte>>();

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

                    tablicaSzachowan[i, j] = lista.GroupBy(a => new { a.X, a.Y }).Select(a => a.Last()).ToList();
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
    }
}
