using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    class Process
    {
        public String   FileName  { get; protected set; }
        public String[] Arguments { get; protected set; }
        public ILogger  Logger    { get; protected set; }

        public String Output
        {
            get { return outputBuffer.ToString(); }
        }

        public Process(String fileName, params String[] arguments)
        {
            FileName  = fileName;
            Arguments = arguments;
            Logger    = null;
        }

        public Process(String fileName, ILogger logger, params String[] arguments)
        {
            FileName  = fileName;
            Arguments = arguments;
            Logger    = logger;
        }

        public int Run()
        {
            int result = -1;

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = FileName;

            StringBuilder buffer = new StringBuilder();
            for(int i=0;i<Arguments.Length;i++)
            {
                buffer.Append(Arguments[i]);
                if(i < Arguments.Length -1)
                {
                    buffer.Append(' ');
                }
            }

            process.StartInfo.Arguments              = buffer.ToString();
            process.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute        = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.OutputDataReceived += CaptureOutput;

            if(Logger != null)
            {
                Logger.Log(String.Format("executing: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments));
            }

            outputBuffer.Clear();
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            result = process.ExitCode;

            if (Logger != null)
            {
                Logger.Log(Output);
                Logger.Log(String.Format("{0} exited with code: {1}", process.StartInfo.FileName, result));
            }

            return result;
        }

        private void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            var str = "" + e.Data;
            outputBuffer.Append(str);
        }

        private StringBuilder outputBuffer = new StringBuilder();
    }
}
