using Cross.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF_Cross.Functional.Converters
{
    public class PointsToPath : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringBuilder data = new StringBuilder("M");
            List<Point> points = (List<Point>)value;
            
            for (int i = 0; i < points.Count; i++)
            {
                data.Append(points[i].X.ToString());
                data.Append(",");
                data.Append(points[i].Y.ToString());
                data.Append((i != (points.Count - 1) ? " L " : " Z"));
            }

            return data.ToString();
            //return Geometry.Parse(data.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
