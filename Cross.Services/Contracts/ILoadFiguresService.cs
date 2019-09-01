﻿using Cross.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services.Contracts
{
    public interface ILoadFiguresService
    {
        List<Figure> LoadFigures(string filePath);
        
    }
}
