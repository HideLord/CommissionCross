using Cross.Data;
using Cross.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services
{
    public class FigureManip : IFigureManip
    {
        private Point rotatePoint(Point point, Point center, double angle)
        {
            Point p = (Point)point.Clone();
            // translate to radians
            angle = ((angle / 180) * Math.PI);
            // find the sin and cos which will become the new base of the coordinate system
            double cosAngle = Math.Cos(angle);
            double sinAngle = Math.Sin(angle);
            // find the x and y scalars
            double dx = (p.X - center.X);
            double dy = (p.Y - center.Y);
            // translate the point to the new system relative to the given center
            p.X = center.X + (dx * cosAngle - dy * sinAngle);
            p.Y = center.Y + (dx * sinAngle + dy * cosAngle);
            return p;
        }

        public Point FindCenterOfBound(Figure figure)
        {
            Point center = new Point(0, 0);
            double minX = figure.Points[0].X, maxX = figure.Points[0].X, minY = figure.Points[0].Y, maxY = figure.Points[0].Y;
            foreach (var p in figure.Points)
            {
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }
            center.X = (maxX + minX) / 2;
            center.Y = (maxY + minY) / 2;
            return center;
        }

        public Point FindCenterOfMass(Figure figure)
        {
            Point center = new Point(0, 0);
            foreach (var p in figure.Points)
            {
                center.X += p.X;
                center.Y += p.Y;
            }
            center.X /= figure.Points.Count;
            center.Y /= figure.Points.Count;
            return center;
        }

        public Figure Rotate(Figure figure, Point origin, double angle)
        {
            Figure rotatedFig = (Figure)figure.Clone();

            for(int i = 0; i < rotatedFig.Points.Count; i++)
            {
                rotatedFig.Points[i] = rotatePoint(rotatedFig.Points[i], origin, angle);
            }

            return rotatedFig;
        }

        public Figure Translate(Figure figure, double x, double y)
        {
            Figure translatedFigure = (Figure)figure.Clone();

            for (int i = 0; i < translatedFigure.Points.Count; i++)
            {
                translatedFigure.Points[i].X += x;
                translatedFigure.Points[i].Y += y;
            }

            return translatedFigure;
        }

        public Figure Scale(Figure figure, double xCoef, double yCoef)
        {
            Figure scaledFigure = (Figure)figure.Clone();

            for (int i = 0; i < scaledFigure.Points.Count; i++)
            {
                scaledFigure.Points[i].X *= xCoef;
                scaledFigure.Points[i].Y *= yCoef;
            }

            return scaledFigure;
        }
    }
}
