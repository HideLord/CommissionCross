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
            // Multiply the new matrixes and then add the origin
            p.X = center.X + (dx * cosAngle - dy * sinAngle);
            p.Y = center.Y + (dx * sinAngle + dy * cosAngle);
            return p;
        }

        public Point FindCenterOfBound(Figure figure)
        {
            if (figure.Points.Count == 0) return new Point(0,0);
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
            if (figure.Points.Count == 0) return new Point(0, 0);
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
            if (figure.Points.Count == 0) return figure;

            Figure rotatedFig = (Figure)figure.Clone();

            for(int i = 0; i < rotatedFig.Points.Count; i++)
            {
                rotatedFig.Points[i] = rotatePoint(rotatedFig.Points[i], origin, angle);
            }

            return rotatedFig;
        }

        public Figure Translate(Figure figure, double x, double y)
        {
            if (figure.Points.Count == 0) return figure;

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
            if (figure.Points.Count == 0) return figure;

            Figure scaledFigure = (Figure)figure.Clone();

            for (int i = 0; i < scaledFigure.Points.Count; i++)
            {
                scaledFigure.Points[i].X *= xCoef;
                scaledFigure.Points[i].Y *= yCoef;
            }

            return scaledFigure;
        }
        public Figure UniformScale(Figure figure, double xCoef, double yCoef)
        {
            if (figure.Points.Count == 0) return figure;

            Figure scaledFigure = (Figure)figure.Clone();

            Point origin = FindCenterOfBound(scaledFigure);

            scaledFigure = Translate(scaledFigure, -origin.X, -origin.Y);

            for (int i = 0; i < scaledFigure.Points.Count; i++)
            {
                scaledFigure.Points[i].X *= xCoef;
                scaledFigure.Points[i].Y *= yCoef;
            }

            scaledFigure = Translate(scaledFigure, origin.X, origin.Y);

            return scaledFigure;
        }

        public Figure NormalizeTranslate(Figure figure)
        {
            var normalizedFig = (Figure)figure.Clone();
            double minX = 1000000, minY = 10000000;
            foreach (var p in normalizedFig.Points)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
            }
            for (int j = 0; j < normalizedFig.Points.Count; j++)
            {
                normalizedFig.Points[j].X -= minX;
                normalizedFig.Points[j].Y -= minY;
            }
            return normalizedFig;
        }
        public List<Figure> Normalize(List<Figure> figures, double defaultSide)
        {
            List<Figure> newFigures = new List<Figure>();
            for (int i = 0; i < figures.Count; i++) newFigures.Add((Figure)figures[i].Clone());

            for (int i = 0; i < newFigures.Count; i++)
            {
                double minX = 1000000, minY = 10000000, maxY = -1000000, maxX = -1000000, currSide;
                foreach (var p in newFigures[i].Points)
                {
                    minX = Math.Min(minX, p.X);
                    minY = Math.Min(minY, p.Y);
                    
                    maxX = Math.Max(maxX, p.X);
                    maxY = Math.Max(maxY, p.Y);
                }
                currSide = Math.Max(maxY - minY, maxX - minX);
                for (int j = 0; j < newFigures[i].Points.Count; j++)
                {
                    newFigures[i].Points[j].X -= minX;
                    newFigures[i].Points[j].Y -= minY;
                }
                if (defaultSide != 0)
                {
                    double coef = defaultSide / currSide;
                    for (int j = 0; j < newFigures[i].Points.Count; j++)
                    {
                        newFigures[i].Points[j].X *= coef;
                        newFigures[i].Points[j].Y *= coef;
                    }
                }
            }

            return newFigures;
        }

        public double Width(Figure figure)
        {
            double minX = 1000000, maxX = -1000000;
            foreach (var p in figure.Points)
            {
                minX = Math.Min(minX, p.X);
                maxX = Math.Max(maxX, p.X);
            }
            return maxX - minX;
        }

        public double Height(Figure figure)
        {
            double minY = 1000000, maxY = -1000000;
            foreach (var p in figure.Points)
            {
                minY = Math.Min(minY, p.Y);
                maxY = Math.Max(maxY, p.Y);
            }
            return maxY - minY;
        }

        
    }
}
