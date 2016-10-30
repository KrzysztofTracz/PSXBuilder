using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class RebuildProgram : ApplicationFramework.Program<PSXBuilder>
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            if(Application.Program<CleanProgram>().Start(arguments))
            {
                result = Application.Program<BuildProgram>().Start(arguments);
            }

            return result;
        }

        protected override String[] GetArguments()
        {
            return new[] { "projectFile", "configuration", "toolsVersion" };
        }

        protected override String GetDescription()
        {
            return "rebuild project";
        }

        protected override String GetSpecifier()
        {
            return "-r";
        }
    }
}
