using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class Point : ICloneable
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point(Point P)
        {
            X = P.X;
            Y = P.Y;
        }

        public object Clone()
        {
            return new Point(this.X, this.Y);
        }
    }
}
