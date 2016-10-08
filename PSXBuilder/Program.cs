using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using System.Diagnostics;

namespace PSXBuilder
{
    class Program
    {
        public enum Settings
        {
            PSXBuildMachine,
            PSXBuildMachineUsername,
            PSXBuildMachinePassword,
            PSToolsPath
        }

        static void Main(string[] args)
        {
            Console.WriteLine("PSXBuilder {0}", Stringify(args));
            Console.WriteLine("----------");
            bool displayHelp = true;
            if(args.Length > 0)
            {
                /////////////////////////////////////////////
                /// -b build
                /////////////////////////////////////////////
                if (args[0] == "-b")
                {
                    displayHelp = false;
                }
                /////////////////////////////////////////////
                /// -r rebuild
                /////////////////////////////////////////////
                else if (args[0] == "-r")
                {
                    displayHelp = false;
                }
                /////////////////////////////////////////////
                /// -c clean
                /////////////////////////////////////////////
                else if (args[0] == "-c")
                {
                    displayHelp = false;
                }
                /////////////////////////////////////////////
                /// -s builder setup
                /////////////////////////////////////////////
                else if (args[0] == "-s")
                {
                    var settings = Enum.GetValues(typeof(Settings)) as Settings[];
                    if (args.Length == settings.Length + 1)
                    {
                        for(int i=0; i< settings.Length; i++)
                        {
                            Properties.Settings.Default[settings[i].ToString()] = args[i + 1];
                        }
                        Properties.Settings.Default.Save();           
                        displayHelp = false;
                    }
                }
                /////////////////////////////////////////////
                /// -d displays builder settings
                /////////////////////////////////////////////
                else if (args[0] == "-d")
                {
                    var settings = Enum.GetValues(typeof(Settings)) as Settings[];
                    for (int i = 0; i < settings.Length; i++)
                    {
                        var settingsName = settings[i].ToString();
                        Console.WriteLine("\t{0}: {1}", settingsName, Properties.Settings.Default[settingsName]);
                    }
                    displayHelp = false;
                }
                /////////////////////////////////////////////
                /// -t build machine connection test
                /////////////////////////////////////////////
                else if (args[0] == "-t")
                {
                    PSTools.Exec("cmd /c systeminfo");
                    displayHelp = false;
                }
            }
           
            if(displayHelp)
            {
                DisplayHelp();
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("\t-h\tdisplays help");
            Console.WriteLine("");
            Console.WriteLine("\t-b\t<ProjectFile>");
            Console.WriteLine("\t\tbuild project");
            Console.WriteLine("");
            Console.WriteLine("\t-r\t<ProjectFile>");
            Console.WriteLine("\t\trebuild project");
            Console.WriteLine("");
            Console.WriteLine("\t-c\t<ProjectFile>");
            Console.WriteLine("\t\tclean project");
            Console.WriteLine("");
            Console.WriteLine("\t-s\t{0}", GetSettingsAsArguments());
            Console.WriteLine("\t\tBuilder setup");
            Console.WriteLine("");
            Console.WriteLine("\t-d\tdisplays builder settings");
            Console.WriteLine("");
            Console.WriteLine("\t-t\tbuild machine connection test");
            Console.WriteLine("");
        }

        static String Stringify(params String[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var arg in args)
            {
                sb.Append(arg);
                sb.Append(" ");
            }
            return sb.ToString();
        }

        static String GetSettingsAsArguments()
        {
            StringBuilder sb = new StringBuilder();
            var settings = Enum.GetValues(typeof(Settings)) as Settings[];
            for (int i = 0; i < settings.Length; i++)
            {
                sb.Append("<");
                sb.Append(settings[i].ToString());
                sb.Append("> ");
            }
            return sb.ToString();
        }
    }
}
