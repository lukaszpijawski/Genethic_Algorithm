using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithms
{
    public class Point<T>
    {
        public T X { get; set; }
        public T Y { get; set; }
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
}
