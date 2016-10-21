using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApplicationFramework
{
    public class Application
    {
        public String  Name    { get; protected set; }
        public Console Console { get; protected set; }

        public Dictionary<Type,IProgram> Programs { get; protected set; }

        public Application(String name)
        {
            Name      = name;
            Console   = new Console();
            _settings = new Settings();
        }

        public void Initialize()
        {
            InitializePrograms();
            _settings.Initialize(this, Console);
            LoadSettings();
            SaveSettings();
            LoadSettings();
        }

        public bool Start(String[] arguments)
        {
            bool result = true;

            Console.WriteLine("{0} {1}", Name, Utils.ConcatArguments(" ", arguments));
            Console.WriteLineSeparator();

            var displayHelp = true;

            if(arguments.Length > 0)
            {
                var specifier = arguments[0];
                var programs  = Programs.Values;

                foreach (var program in programs)
                {
                    if(program.Specifier == specifier)
                    {
                        if(program.Arguments.Length == arguments.Length - 1)
                        {
                            displayHelp = false;
                            result = program.Start(GetProgramArguments(arguments));
                        }
                        break;
                    }
                }
            }

            if(displayHelp)
            {
                DisplayHelp();
            }

            return result;
        }

        public void DisplayHelp()
        {
            Console.PushTab();

            var programs = Programs.Values;
            foreach (var program in programs)
            {
                Console.WriteLine("{0}\t{1}", program.Specifier, 
                                              Utils.ConcatArguments(" ", program.Arguments));
                Console.PushTab();
                Console.WriteLine(program.Description);
                Console.PopTab();

                Console.WriteLine();
            }

            Console.PopTab();
        }

        public T  Program<T>() where T : IProgram
        {
            T result = null;

            if(Programs.ContainsKey(typeof(T)))
            {
                result = Programs[typeof(T)] as T;
            }

            return result;
        }

        protected String[] GetProgramArguments(params String[] arguments)
        {
            var result = new String[arguments.Length - 1];

            for(int i=0;i< result.Length;i++)
            {
                result[i] = arguments[i + 1];
            }

            return result;
        }

        protected void InitializePrograms()
        {
            Programs = new Dictionary<Type, IProgram>();

            var assembly = Assembly.GetEntryAssembly();
            var types    = assembly.GetTypes();

            foreach (var type in types)
            {
                if(type.IsClass && type.IsSubclassOf(typeof(IProgram)))
                {
                    if(type.BaseType.GetGenericArguments().Contains(this.GetType()))
                    {
                        var contructor = type.GetConstructor(new Type[] { });
                        var program = contructor.Invoke(new object[] { }) as IProgram;
                        program.Initialize(this);
                        Programs.Add(type, program);
                    }
                }
            }
        }

        protected bool LoadSettings()
        {
            return _settings.Load();
        }

        protected bool SaveSettings()
        {
            return  _settings.Save();
        }

        private Settings _settings = null;
    }
}
