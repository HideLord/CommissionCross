using Cross.Data;
using Cross.Services.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cross.Services
{
    public class LoadFiguresService : ILoadFiguresService
    {
        public List<Figure> LoadFigures(string filePath)
        {
            List<Figure> figures = new List<Figure>();
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                int numLines = 0;
                if (!int.TryParse(lines[i], out numLines)) break;
                figures.Add(new Figure());
                while (numLines > 0)
                {
                    numLines--;
                    i++;
                    Regex r = new Regex(" +|\t");
                    string[] nums = r.Split(lines[i]);
                    double x, y;
                    x = double.Parse(nums[0]);
                    y = double.Parse(nums[1]);
                    figures.Last().Points.Add(new Point(x, y));
                }
            }
            return figures;
        }
    }
}
