using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Cross.Data
{
    public class FormData : ICloneable, INotifyPropertyChanged
    {
        private int squareWidth = 100;
        public int SquareWidth {
            get => squareWidth;
            set
            {
                if (squareWidth == value) return;
                squareWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareWidth"));
            }
        }
        private int squareHeight = 100;
        public int SquareHeight
        {
            get => squareHeight;
            set
            {
                if (squareHeight == value) return;
                squareHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareHeight"));
            }
        }

        private int setWidth = 100;
        public int SetWidth
        {
            get => setWidth;
            set
            {
                if (setWidth == value) return;
                setWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetWidth"));
            }
        }
        private int setHeight = 100;
        public int SetHeight
        {
            get => setHeight;
            set
            {
                if (setHeight == value) return;
                setHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetHeight"));
            }
        }

        private int arrowWidth = 100;
        public int ArrowWidth
        {
            get => arrowWidth;
            set
            {
                if (arrowWidth == value) return;
                arrowWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ArrowWidth"));
            }
        }
        private int arrowHeight = 100;
        public int ArrowHeight
        {
            get => arrowHeight;
            set
            {
                if (arrowHeight == value) return;
                arrowHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ArrowHeight"));
            }
        }

        private int squareRotation = 0;
        public int SquareRotation
        {
            get => squareRotation;
            set
            {
                if (squareRotation == value) return;
                squareRotation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareRotation"));
            }
        }
        private int setRotation = 0;
        public int SetRotation
        {
            get => setRotation;
            set
            {
                if (setRotation == value) return;
                setRotation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetRotation"));
            }
        }

        public int squareIndex  = 0;
        public int SquareIndex
        {
            get => squareIndex;
            set
            {
                if (squareIndex == value) return;
                squareIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareIndex"));
            }
        }
        public int setIndex= 0;
        public int SetIndex
        {
            get => setIndex;
            set
            {
                if (setIndex == value) return;
                setIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetIndex"));
            }
        }
        public int arrowIndex = 0;
        public int ArrowIndex
        {
            get => arrowIndex;
            set
            {
                if (arrowIndex == value) return;
                arrowIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ArrowIndex"));
            }
        }

        public Color squareColor = Colors.White;
        public Color SquareColor
        {
            get => squareColor;
            set
            {
                if (squareColor == value) return;
                squareColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SquareColor"));
            }
        }
        public Color setColor = Colors.White;
        public Color SetColor
        {
            get => setColor;
            set
            {
                if (setColor == value) return;
                setColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetColor"));
            }
        }
        public Color arrowColor  = Colors.Black;
        public Color ArrowColor
        {
            get => arrowColor;
            set
            {
                if (arrowColor == value) return;
                arrowColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ArrowColor"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
