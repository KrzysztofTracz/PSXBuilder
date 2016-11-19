using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public abstract class IProgram
    {
        public String   Specifier   { get; protected set; }
        public String   Description { get; protected set; }

        public abstract void Initialize(Application application);
        public abstract bool Start(params String[] arguments);
        public abstract bool Start();
        public abstract void DisplayHelp();

        public int ArgumentsCount
        {
            get
            {
                var result = 0;
                foreach(var argument in Arguments)
                {
                    if(!argument.Optional) ++result;
                }
                return result;
            }
        }
        
        protected abstract String GetSpecifier();
        protected abstract String GetDescription();
        protected abstract void   Log(String format, params object[] objects);

        protected void Initialize()
        {
            var specifier = GetSpecifier();
            Specifier = !String.IsNullOrEmpty(specifier) ? Utils.Text(ProgramArgument.Prefix, specifier) : "";
            
            Description = GetDescription();
            InitializeArguments();
        }
                
        protected virtual void InitializeArguments()
        {
            Arguments = new List<ProgramArgument>();

            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(ProgramArgumentAttribute), true);
                if (attributes.Length > 0)
                {
                    var programArgumentType = ProgramArgument.TypeToProgramArgumentType(field.FieldType);
                    if (programArgumentType != ProgramArgumentType.Unknown)
                    {
                        var programArgument = new ProgramArgument(attributes[0] as ProgramArgumentAttribute,
                                                                  field,
                                                                  this);
                        Arguments.Add(programArgument);
                    }
                }
            }
        }

        protected List<ProgramArgument> Arguments = null;
    }

    public abstract class Program<T> : IProgram 
        where T : Application
    {
        public T Application { get; protected set; }

        public override void Initialize(Application application)
        {
            Application = application as T;
            Initialize();
        }

        public override bool Start(params String[] arguments)
        {
            var result = true;
            var unusedArguments = new List<String>(arguments);

            foreach (var programArgument in Arguments)
            {
                bool argumentFound = false;
                foreach (var argument in arguments)
                {
                    if (argument.StartsWith(programArgument.Name))
                    {
                        if (programArgument.Type == ProgramArgumentType.Bool &&
                           programArgument.Name == argument)
                        {
                            programArgument.Value = "true";
                            argumentFound = true;
                        }
                        else
                        {
                            try
                            {
                                programArgument.Value = argument.Substring(programArgument.Name.Length + 1);
                                argumentFound = true;
                            }
                            catch (System.Exception)
                            {
                                Log("Invalid value of {0}.", programArgument.Name);
                            }
                        }
                        unusedArguments.Remove(argument);
                        break;
                    }
                }

                if (!argumentFound && !programArgument.Optional)
                {
                    Log("Missing argument {0}.", programArgument.Name);
                    result = false;
                    break;
                }
            }

            if (unusedArguments.Count > 0)
            {
                foreach (var argument in unusedArguments)
                {
                    Log("Invalid argument {0}.", argument);
                }
                result = false;
            }

            if(!result)
            {
                DisplayHelp();
            }
            else
            {
                result = Start();
            }

            return result;
        }

        public override void DisplayHelp()
        {
            Application.Console.WriteLine("{0}:\t{1}", Specifier, Description);
            var arguments = GetArguments();
            if (arguments.Length > 0)
            {
                Application.Console.PushTab();
                foreach (var argument in arguments)
                {
                    Application.Console.WriteLine(argument);
                }
                Application.Console.PopTab();
            }

        }

        protected override void Log(String format, params object[] objects)
        {
            Application.Console.Log(format, objects);
        }

        protected virtual String[] GetArguments()
        {
            var result = new String[Arguments.Count];

            for(int i=0; i<Arguments.Count; i++)
            {
                var argument = Arguments[i];
                var buffer   = new StringBuilder();

                if (argument.Optional) buffer.Append("? ");

                buffer.Append(argument.Name);

                if (!String.IsNullOrEmpty(argument.DefaultValue))
                {
                    buffer.Append('=');
                    buffer.Append(argument.DefaultValue);
                }

                result[i] = buffer.ToString();
            }
            return result;
        }
    }
}
