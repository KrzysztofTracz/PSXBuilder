using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public class Process
    {
        public String   FileName  { get; protected set; }
        public String[] Arguments { get; protected set; }

        public String Output
        {
            get { return outputBuffer.ToString(); }
        }

        public Process(String fileName, params String[] arguments)
        {
            FileName  = fileName;
            Arguments = arguments;
        }

        public int Run(ILogger logger = null)
        {
            int result = -1;

            var process = new System.Diagnostics.Process();

            process.StartInfo.FileName               = FileName;
            process.StartInfo.Arguments              = Utils.ConcatArguments(" ", Arguments);
            process.StartInfo.WindowStyle            = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute        = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived              += CaptureOutput;

            if(logger != null)
            {
                logger.Log(String.Format("executing: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments));
            }

            outputBuffer.Clear();
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            result = process.ExitCode;

            if (logger != null)
            {
                logger.Log(Output);
                logger.Log(String.Format("{0} exited with code: {1}", process.StartInfo.FileName, result));
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
