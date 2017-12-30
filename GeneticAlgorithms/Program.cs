using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    class Program
    {
        static List<Point<byte>>[,] tablicaSzachowanPoPrzekatnej;

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

        static void PrintArrayOfListsOfPoints(List<Point<byte>>[,] array)
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

        class PointEqualityComparer<T> : IEqualityComparer<Point<T>> where T : struct
        {
            public bool Equals(Point<T> x, Point<T> y)
            {
                return x.X.Equals(y.X) && x.Y.Equals(y.Y);
            }

            public int GetHashCode(Point<T> obj)
            {
                return obj.GetHashCode();
            }
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

        static void Main(string[] args)
        {
            tablicaSzachowanPoPrzekatnej = StworzTabliceSzachowanPoPrzekatnej(8);
            Console.WriteLine(ObliczLiczbeSzachowan(new byte[] { 1, 0, 2, 2, 5, 4, 6, 3}));
        }
    }
}
