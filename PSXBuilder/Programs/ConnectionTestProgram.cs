using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class ConnectionTestProgram : ApplicationFramework.Program
    {
        public override bool Start(params String[] arguments)
        {
            PSTools.CMD("systeminfo", false);
            return true;
        }

        protected override String[] GetArguments()
        {
            return new String[] { };
        }

        protected override String GetDescription()
        {
            return "build machine connection test";
        }

        protected override String GetSpecifier()
        {
            return "-t";
        }
    }
}
