using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class FormData : BaseNotify
    {
        public int SquareWidth { get; set; } = 100;
        public int SquareHeight { get; set; } = 100;

        public int SetWidth { get; set; } = 100;
        public int SetHeight { get; set; } = 100;

        public int ArrowWidth { get; set; } = 100;
        public int ArrowHeight { get; set; } = 100;

        public int SquareRotation { get; set; } = 0;
        public int SetRotation { get; set; } = 0;

        public int SquareIndex { get; set; }
        public int SetIndex { get; set; }
        public int ArrowIndex { get; set; }
    }
}
