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

        public Dictionary<Type,Program> Programs { get; protected set; }

        public Application(String name)
        {
            InitializePrograms();
            Name    = name;
            Console = new Console();
        }

        public void Start(String[] arguments)
        {
            Console.WriteLine("{0} {1}", Name, ConcatArguments(" ", arguments));
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
                            program.Start(GetProgramArguments(arguments));
                        }
                        break;
                    }
                }
            }

            if(displayHelp)
            {
                DisplayHelp();
            }
        }

        public void DisplayHelp()
        {
            Console.PushTab();

            var programs = Programs.Values;
            foreach (var program in programs)
            {
                Console.WriteLine("{0}\t{1}", program.Specifier, 
                                              ConcatArguments(" ", program.Arguments));
                Console.PushTab();
                Console.WriteLine(program.Description);
                Console.PopTab();

                Console.WriteLine();
            }

            Console.PopTab();
        }

        public T  Program<T>() where T : Program
        {
            T result = null;

            if(Programs.ContainsKey(typeof(T)))
            {
                result = Programs[typeof(T)] as T;
            }

            return result;
        }

        public String ConcatArguments(String separator, params String[] arguments)
        {
            String result = "";
            if (arguments != null)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < arguments.Length; i++)
                {
                    sb.Append(arguments[i]);
                    if (i < arguments.Length - 1)
                    {
                        sb.Append(separator);
                    }
                }
                result = sb.ToString();
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
            Programs = new Dictionary<Type, Program>();

            var assembly = Assembly.GetEntryAssembly();
            var types    = assembly.GetTypes();

            foreach (var type in types)
            {
                if(type.IsClass && type.IsSubclassOf(typeof(Program)))
                {
                    var contructor = type.GetConstructor(new Type[] { });
                    var program = contructor.Invoke(new object[] { }) as Program;
                    program.Initialize(this);
                    Programs.Add(type, program);
                }
            }
        }
    }
}
