using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    public abstract class Program
    {
        public String   Specifier   { get; protected set; }
        public String   Description { get; protected set; }
        public String[] Arguments   { get; protected set; }

        public Application Application { get; protected set; }

        public void Initialize(Application application)
        {
            Application = application;
            Specifier   = GetSpecifier();
            Description = GetDescription();
            Arguments   = GetArguments();
        }

        public abstract bool Start(params String[] arguments);

        protected abstract String   GetSpecifier();
        protected abstract String   GetDescription();
        protected abstract String[] GetArguments();
    }
}
