using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class SizePackage : ICloneable
    {
        public int SquareWidth { get; set; } = 100;
        public int SquareHeight { get; set; } = 100;

        public int SetWidth { get; set; } = 100;
        public int SetHeight { get; set; } = 100;

        public int ArrowWidth { get; set; } = 100;
        public int ArrowHeight { get; set; } = 100;

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
