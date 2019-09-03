using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Cross.Data
{
    public class FormData : BaseNotify, ICloneable
    {
        public int SquareWidth { get; set; } = 100;
        public int SquareHeight { get; set; } = 100;

        public int SetWidth { get; set; } = 100;
        public int SetHeight { get; set; } = 100;

        public int ArrowWidth { get; set; } = 100;
        public int ArrowHeight { get; set; } = 100;

        public int SquareRotation { get; set; } = 0;
        public int SetRotation { get; set; } = 0;

        public int SquareIndex { get; set; } = 0;
        public int SetIndex { get; set; } = 0;
        public int ArrowIndex { get; set; } = 0;

        public Color SquareColor { get; set; } = Colors.White;
        public Color SetColor { get; set; } = Colors.White;
        public Color ArrowColor { get; set; } = Colors.Black;

        public object Clone()
        {
            var newData = new FormData();
            newData.SquareWidth = SquareWidth;
            newData.SquareHeight = SquareHeight;
            newData.SetWidth = SetWidth;
            newData.SetHeight = SetHeight;
            newData.ArrowWidth = ArrowWidth;
            newData.ArrowHeight = ArrowHeight;
            newData.SquareIndex = SquareIndex;
            newData.SetIndex = SetIndex;
            newData.ArrowIndex = ArrowIndex;
            newData.SquareRotation = SquareRotation;
            newData.SetRotation = SetRotation;
            newData.SquareColor = SquareColor;
            newData.SetColor = SetColor;
            newData.ArrowColor = ArrowColor;
            return newData;
        }
    }
}
