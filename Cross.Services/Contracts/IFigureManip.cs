using Cross.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services.Contracts
{
    public interface IFigureManip
    {
        List<Figure> Normalize(List<Figure> figures, double defaultSide);
        Figure Rotate(Figure figure, Point origin, double angle);
        Figure Scale(Figure figure, double xCoef, double yCoef);
        Figure UniformScale(Figure figure, double xCoef, double yCoef);
        Figure Translate(Figure figure, double x, double y);
        Point FindCenterOfMass(Figure figure);
        Point FindCenterOfBound(Figure figure);
    }
}
