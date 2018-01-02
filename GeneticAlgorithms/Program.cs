using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    class Program
    {
        #region Pola statyczne
        static List<Point<byte>>[,] tablicaSzachowanPoPrzekatnej;
        static int _liczbaHetmanow;
        static float _maksymalnaLiczbaSzachowan;
        #endregion

        #region Metody pomocnicze do obliczenia przystosowania
        static float ObliczPrzystosowanie(byte[] osobnik)
        {
            _maksymalnaLiczbaSzachowan = (float)(_liczbaHetmanow - 1) * (float)_liczbaHetmanow / 2.0f;
            tablicaSzachowanPoPrzekatnej = StworzTabliceSzachowanPoPrzekatnej(_liczbaHetmanow);
            var liczbaSzachowan = ObliczLiczbeSzachowan(osobnik);
            if (liczbaSzachowan == 0)
            {
                return 1.0f;
            }
            return ((float)_maksymalnaLiczbaSzachowan - (float)liczbaSzachowan) / (float)_maksymalnaLiczbaSzachowan;
        }

        static List<Point<byte>>[,] StworzTabliceSzachowanPoPrzekatnej(int rozmiar)
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

        static int ObliczLiczbeSzachowan(byte[] osobnik)
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

        static int ObliczLiczbeSzachowanPoPrzekatnej(List<Point<byte>> osobnikJakoLista)
        {
            int liczbaSzachowan = 0;
            foreach (var hetman in osobnikJakoLista)
            {
                liczbaSzachowan += tablicaSzachowanPoPrzekatnej[hetman.X, hetman.Y].Where(a => osobnikJakoLista.Contains(a, new PointEqualityComparer<byte>())).Count() - 1;
            }

            return liczbaSzachowan / 2;
        }        

        static int ObliczLiczbeSzachowanPoKolumnie(byte[] osobnik)
        {
            int liczbaSzachowan = 0;
            foreach (var hetman in osobnik)
            {
                liczbaSzachowan += osobnik.Where(a => a == hetman).Count() - 1;
            }

            return liczbaSzachowan / 2;
        }      
        
        static void WyswietlOsobnika(byte[] osobnik)
        {
            foreach (var el in osobnik)
            {
                Console.Write((el + 1) + " ");
            }
        }

        static void WyswietlTabliceListPunktów(List<Point<byte>>[,] array)
        {
            for (byte i = 0; i < array.GetLength(0); i++)
            {
                for (byte j = 0; j < array.GetLength(1); j++)
                {
                    Console.Write("[{0}, {1}]:\t", i, j);
                    foreach (var point in array[i, j])
                    {
                        Console.Write("[{0}, {1}]", point.X, point.Y);
                    }
                    Console.WriteLine();
                }
            }
        }
        #endregion

        static void Main(string[] args)
        {
            _liczbaHetmanow = 8;
            NHetmanow nHetmanow = new NHetmanow(_liczbaHetmanow, 100, 0.01f);
            var wynik = nHetmanow.Szukaj(100);
            Console.WriteLine("Przystosowanie wyniku: " + ObliczPrzystosowanie(wynik));
            WyswietlOsobnika(wynik);
            Console.WriteLine();
        }
    }
}
