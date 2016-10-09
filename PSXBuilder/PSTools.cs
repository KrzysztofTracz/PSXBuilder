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
        public static String Exec(String arguments, bool silent = true)
        {
            return Execute(silent, arguments);
        }

        public static String CMD(String arguments, bool silent = true)
        {
            return Execute(silent, "cmd", "/c", arguments);            
        }

        public static PSTools Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new PSTools();
                }
                return _instance;
            }
        }

        private static String Execute(bool silent, params String[] arguments)
        {
            var s = Instance.Silent;
            Instance.Silent = silent;

            var result = Instance.Execute(EPsTool.PsExec, arguments);
            Instance.Silent = s;

            return result;
        }

        public bool Silent { get; set; }

        private enum EPsTool
        {
            PsExec,
        }

        private PSTools()
        {
            Silent = true;
        }

        private static PSTools _instance = null;

        private String Execute(EPsTool tool, params String[] arguments)
        {
            String result = "";

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

            if (!Silent)
            {
                Console.WriteLine("executing: {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);
            }

            outputBuffer.Clear();
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            result = outputBuffer.ToString();
            outputBuffer.Clear();
            return result;
        }

        private String Path
        {
            get { return Properties.Settings.Default[Program.Settings.PSToolsPath.ToString()] as String; }
        }

        private String ConnectionParams
        {
            get
            {
                return String.Format("\\\\{0} -u {1} -p {2} -nobanner -accepteula",
                                     Properties.Settings.Default[Program.Settings.PSXBuildMachine.ToString()] as String,
                                     Properties.Settings.Default[Program.Settings.PSXBuildMachineUsername.ToString()] as String,
                                     Properties.Settings.Default[Program.Settings.PSXBuildMachinePassword.ToString()] as String);
            }
        }
    
        private void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            var str = "" + e.Data;
            outputBuffer.Append(str);
            if(!Silent)
            {
                Console.WriteLine(str);
            }
        }

        private StringBuilder outputBuffer = new StringBuilder();
    }
}
