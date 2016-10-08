using System;
using System.Text;

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
                var specifier = args[0];
                if (specifier == "-b")
                {
                    displayHelp = !Build(args);
                }
                else if (specifier == "-r")
                {
                    if(Clean(args))
                    {
                        displayHelp = !Build(args);
                    }
                }
                else if (specifier == "-c")
                {
                    displayHelp = !Clean(args);
                }
                else if (specifier == "-s")
                {
                    displayHelp = !Setup(args);
                }
                else if (specifier == "-d")
                {
                    displayHelp = !DisplaySettings();
                }
                else if (specifier == "-t")
                {                   
                    displayHelp = !Test();
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
            Console.WriteLine("\t-b\t<ProjectFile> <configuration> <toolsVersion>");
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

        static bool Test()
        {
            PSTools.Exec("cmd /c systeminfo");
            return true;
        }

        static bool DisplaySettings()
        {
            var settings = Enum.GetValues(typeof(Settings)) as Settings[];
            for (int i = 0; i < settings.Length; i++)
            {
                var settingsName = settings[i].ToString();
                Console.WriteLine("\t{0}: {1}", settingsName, Properties.Settings.Default[settingsName]);
            }
            return true;
        }

        static bool Setup(String[] args)
        {
            bool result = false;

            var settings = Enum.GetValues(typeof(Settings)) as Settings[];
            if (args.Length == settings.Length + 1)
            {
                for (int i = 0; i < settings.Length; i++)
                {
                    Properties.Settings.Default[settings[i].ToString()] = args[i + 1];
                }
                Properties.Settings.Default.Save();
                result = true;
            }

            return result;
        }

        static bool Build(String[] args)
        {
            bool result = false;

            if (args.Length == 4)
            {
                var project = new PSXProject();
                if (project.Load(args[1], args[2], args[3]))
                {
                    result = true;
                }
            }

            return result;
        }

        static bool Clean(String[] args)
        {
            bool result = false;

            return result;
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
