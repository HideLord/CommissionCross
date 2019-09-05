using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services.Contracts
{
    public interface IProcessService
    {
        StreamWriter SpawnProcess(string filePath, DataReceivedEventHandler handler);
    }
}
