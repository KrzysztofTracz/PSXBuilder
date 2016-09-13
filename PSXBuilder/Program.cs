using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;

namespace PSXBuilder
{
    class Program
    {
        enum Settings
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
                if(args[0] == "-b")
                {
                    displayHelp = false;
                }
                else if (args[0] == "-r")
                {
                    displayHelp = false;
                }
                else if (args[0] == "-c")
                {
                    displayHelp = false;
                }
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
            }
           
            if(displayHelp)
            {
                DisplayHelp();
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("\t-h\tDisplay help.");
            Console.WriteLine("");
            Console.WriteLine("\t-b\t<ProjectFile>");
            Console.WriteLine("\t\tBuild project.");
            Console.WriteLine("");
            Console.WriteLine("\t-r\t<ProjectFile>");
            Console.WriteLine("\t\tRebuild project.");
            Console.WriteLine("");
            Console.WriteLine("\t-c\t<ProjectFile>");
            Console.WriteLine("\t\tClean project.");
            Console.WriteLine("");
            Console.WriteLine("\t-s\t{0}", GetSettingsAsArguments());
            Console.WriteLine("\t\tSetup build settings.");
            Console.WriteLine("");
            Console.WriteLine("\t-d\tDisplay build settings.");
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
