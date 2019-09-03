using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class SizePackage : ICloneable
    {
        public double SquareWidth { get; set; }
        public double SquareHeight { get; set; }

        public double SetWidth { get; set; }
        public double SetHeight { get; set; }

        public double ArrowWidth { get; set; }
        public double ArrowHeight { get; set; }

        public SizePackage(){ }
        public SizePackage(SizePackage data)
        {
            SquareHeight = data.SquareHeight;
            SquareWidth = data.SquareWidth;
            SetHeight = data.SetHeight;
            SetWidth = data.SetWidth;
            ArrowHeight = data.ArrowHeight;
            ArrowWidth = data.ArrowWidth;
        }

        public object Clone()
        {
            return new SizePackage(this);
        }
    }
}
