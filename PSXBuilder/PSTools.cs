using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSXBuilder
{
    class PSTools
    {
        public static void Exec(params String[] arguments)
        {
            Execute(EPsTool.PsExec, arguments);
        }

        private enum EPsTool
        {
            PsExec,
        }

        private static void Execute(EPsTool tool, params String[] arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = Path + "\\" + tool.ToString() + ".exe";

            StringBuilder buffer = new StringBuilder();
            buffer.Append(ConnectionParams);
            foreach (var argument in arguments)
            {
                buffer.Append(' ');
                buffer.Append(argument);
            }
            process.StartInfo.Arguments = buffer.ToString();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += CaptureOutput;
            Console.WriteLine("executing: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }

        private static String Path
        {
            get { return Properties.Settings.Default[Program.Settings.PSToolsPath.ToString()] as String; }
        }

        private static String ConnectionParams
        {
            get
            {
                return String.Format("\\\\{0} -u {1} -p {2}",
                                     Properties.Settings.Default[Program.Settings.PSXBuildMachine.ToString()] as String,
                                     Properties.Settings.Default[Program.Settings.PSXBuildMachineUsername.ToString()] as String,
                                     Properties.Settings.Default[Program.Settings.PSXBuildMachinePassword.ToString()] as String);
            }
        }

        private static void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("" + e.Data);
        }
    }
}
