using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    public abstract class AlgorytmGenetyczny<TypOsobnika> where TypOsobnika : IList<byte>
    {
        #region Abstrakcyjne chronione pola
        protected TypOsobnika najlepszyOsobnik;
        #endregion

        #region Abstrakcyjne chronione metody
        protected abstract TypOsobnika[] GenerujLosowaPopulacje(int rozmiar);
        protected abstract TypOsobnika Koniec(bool najlepszyMozliwy = false);
        protected abstract float ObliczPrzystosowanie(TypOsobnika osobnik);
        protected abstract void Krzyzuj(TypOsobnika osobnik1, TypOsobnika osobnik2, out TypOsobnika nowyOsobnik1, out TypOsobnika nowyOsobnik2);
        protected abstract TypOsobnika Mutuj(TypOsobnika osobnik);
        #endregion

        #region Własności
        protected int RozmiarPopulacji { get; set; }
        protected float PrawdopodobienstwoMutacji { get; set; }
        #endregion

        #region Szukaj
        public TypOsobnika Szukaj(int liczbaIteracji)
        {
            TypOsobnika[] populacja = GenerujLosowaPopulacje(RozmiarPopulacji);            
            float[] przystosowanie = new float[RozmiarPopulacji];
            int[] rodzice = new int[RozmiarPopulacji];
            float maksymalnePrzystosowanie = 0;
            int indeksNajlepszegoOsobnika = 0;

            while (liczbaIteracji > 0)
            {
                for (int i = 0; i < RozmiarPopulacji; i++)
                {
                    przystosowanie[i] = ObliczPrzystosowanie(populacja[i]);
                    if (przystosowanie[i] > maksymalnePrzystosowanie)
                    {
                        maksymalnePrzystosowanie = przystosowanie[i];
                        indeksNajlepszegoOsobnika = i;
                    }
                }

                najlepszyOsobnik = populacja[indeksNajlepszegoOsobnika];
                TypOsobnika wynik = Koniec();
                if (wynik != null)
                {
                    return wynik;
                }
                LosowanieDoKrzyzowania(przystosowanie, rodzice);
                TypOsobnika[] nowaPopulacja = new TypOsobnika[RozmiarPopulacji];
                for (int i = 0; i < rodzice.Length; i += 2)
                {
                    Krzyzuj(populacja[rodzice[i]], populacja[rodzice[i + 1]],
                       out nowaPopulacja[i], out nowaPopulacja[i + 1]);
                }
                for (int i = 0; i < nowaPopulacja.Length; i++)
                {
                    nowaPopulacja[i] = Mutuj(nowaPopulacja[i]);
                }
                populacja = nowaPopulacja;
                liczbaIteracji--;
            }
            return Koniec(true);
        }
        #endregion

        #region Losowanie do krzyzowania
        void LosowanieDoKrzyzowania(float[] przystosowanie, int[] rodzice)
        {
            Random r = new Random();
            float[] progi = Progi(przystosowanie);
            var iloscElementow = przystosowanie.Length;
            for (int i = 0; i < iloscElementow - 1; i++)
            {
                rodzice[i] = Indeks((float)r.NextDouble(), progi);
                int liczbaIteracji = 100;
                do
                {
                    rodzice[i + 1] = Indeks((float)r.NextDouble(), progi);
                    liczbaIteracji--;
                }
                while (rodzice[i] == rodzice[i + 1] && liczbaIteracji > 0);
            }
        }

        private float[] Progi(float[] przystosowanie)
        {
            float[] progi = new float[przystosowanie.Length];
            float suma = 0;
            for (int i = 0; i < progi.Length; i++)
            {
                suma += przystosowanie[i];
                progi[i] = suma;
            }
            for (int i = 0; i < progi.Length; i++)
            {
                progi[i] /= suma;
            }
            return progi;
        }

        private int Indeks(float x, float[] progi)
        {
            int a = 0, b = progi.Length - 1;
            do
            {
                int c = (a + b) / 2;
                if (c == a)
                {
                    return progi[c] >= x ? c : b;
                }
                if (progi[c] >= x)
                {
                    b = c;
                }
                else
                {
                    a = c;
                }
            } while (a < b);
            return a;
        }
        #endregion

        
    }

    #region TypOsobnikaIEqualityComparer
    public class TypOsobnikaIEqualityComparer<T> : IEqualityComparer<T> where T : IList<byte>
    {
        public bool Equals(T x, T y)
        {
            if (x.Count() != y.Count())
            {
                return false;
            }
            for (int i = 0; i < x.Count(); i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(T obj)
        {
            return -1;
        }
    }
    #endregion
}