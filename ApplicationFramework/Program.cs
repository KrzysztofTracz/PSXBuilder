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
        public String[] Arguments   { get; protected set; }

        public abstract void Initialize(Application application);
        public abstract bool Start     (params String[] arguments);
                
        protected abstract String GetSpecifier();
        protected abstract String GetDescription();
        protected abstract String[] GetArguments();

        protected void Initialize()
        {
            Specifier   = GetSpecifier();
            Description = GetDescription();
            Arguments   = GetArguments();
        }
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

        protected void Log(String format, params object[] objects)
        {
            Application.Console.Log(format, objects);
        }
    }
}
