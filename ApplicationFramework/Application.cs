using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApplicationFramework
{
    public abstract class Application
    {
        public String  Name { get; protected set; }

        public Console  Console  { get; protected set; }
        public Settings Settings { get; protected set; }

        public Dictionary<Type,IProgram> Programs       { get; protected set; }
        public IProgram                  DefaultProgram { get; protected set; }

        public abstract String GetName();

        public static int Start<T>(String[] arguments) where T : Application, new()
        {
            var application = new T();
            application.Initialize();
            return application.Start(arguments) ? 0 : -1;
        }

        public Application()
        {
            Name     = GetName();
            Console  = new Console();
            Settings = new Settings();

            DefaultProgram = null;
        }

        public virtual void Initialize()
        {
            Settings.Initialize(this, Console);
            Settings.Load();

            InitializePrograms(Assembly.GetAssembly(typeof(Application)));
            InitializePrograms(Assembly.GetAssembly(GetType()));
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
                        result      = program.Start(GetProgramArguments(arguments));
                        displayHelp = false;
                        break;
                    }
                }
            }
            else if(DefaultProgram != null)
            {
                DefaultProgram.Start();
            }

            if(displayHelp)
            {
                DisplayHelp();
            }

            return result;
        }

        public void DisplayHelp()
        {
            Console.WriteLine("Usage:");
            Console.PushTab();
            Console.WriteLine("{0} /? /argument /argument=value /argument=value0,value1,value2", Name);
            Console.WriteLine();
            Console.PopTab();

            Console.WriteLine("Options:");
            Console.PushTab();
            var programs = Programs.Values;
            foreach (var program in programs)
            {
                program.DisplayHelp();
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

        protected void InitializePrograms(Assembly assembly)
        {
            if(Programs == null)
            {
                Programs = new Dictionary<Type, IProgram>();
            }

            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if(type.IsClass && type.IsSubclassOf(typeof(IProgram)) && !type.IsAbstract)
                {
                    var baseType = type;
                    bool applicationCheck = false;
                    do
                    {
                        baseType = baseType.BaseType;
                        var genericArguments = baseType.GetGenericArguments();
                        if (genericArguments.Contains(this.GetType()) ||
                            genericArguments.Contains(typeof(Application)))
                        {
                            applicationCheck = true;
                        }
                    }
                    while (!applicationCheck && baseType != typeof(IProgram));

                    if (applicationCheck)
                    {
                        if (!Programs.ContainsKey(type))
                        {
                            var contructor = type.GetConstructor(new Type[] { });
                            var program    = contructor.Invoke(new object[] { }) as IProgram;

                            program.Initialize(this);
                            Programs.Add(type, program);

                            if(String.IsNullOrEmpty(program.Specifier) && 
                               program.ArgumentsCount == 0)
                            {
                                DefaultProgram = program;
                            }
                        }
                    }
                }
            }
        }
    }
}
