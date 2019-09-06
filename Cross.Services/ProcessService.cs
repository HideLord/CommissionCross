//#define DEBUG_OUTPUT
using Cross.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Services
{
    public class ProcessService : IProcessService
    {
        private Process process;

        private string ProcessArgs = "";

        private DataReceivedEventHandler outputHandler = null;

        public void SetHandler(DataReceivedEventHandler handler)
        {
            outputHandler = handler;
        }

        public void SetProcessArgs(string args)
        {
            ProcessArgs = args;
        }

        public void Kill()
        {
            process.Kill();
        }

        public StreamWriter SpawnProcess(string filePath)
        {
#if DEBUG_OUTPUT
            process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.UseShellExecute = false;
            var output = new StringBuilder("");
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.Arguments = this.ProcessArgs;

            process.Start();
            StreamWriter writer = process.StandardInput;
            return writer;
#else
            process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            var output = new StringBuilder("");
            process.OutputDataReceived += new DataReceivedEventHandler(this.outputHandler);
            process.StartInfo.Arguments = this.ProcessArgs;
            process.StartInfo.RedirectStandardInput = true;

            process.Start();
            StreamWriter writer = process.StandardInput;
            process.BeginOutputReadLine();
            return writer;
#endif
        }
        ~ProcessService()
        {
            process.Kill();
        }

    }
}
