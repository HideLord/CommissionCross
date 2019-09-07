using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Data
{
    public class Figure : INotifyPropertyChanged, ICloneable
    {
        private List<Point> points; 
        public List<Point> Points
        {
            get => points;
            set
            {
                points = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Points"));
            }
        }

        public Figure()
        {
            Points = new List<Point>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Clone()
        {
            var clone = new Figure();
            foreach(var p in Points)
            {
                clone.Points.Add((Point)p.Clone());
            }
            return clone;
        }
    }
}
