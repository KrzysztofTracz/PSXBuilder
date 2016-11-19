using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;

namespace PSXBuilder
{
    class RebuildProgram : ProjectProgram
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

        public override bool Start()
        {
            return false;
        }

        protected override String GetDescription()
        {
            return "Rebuild project";
        }

        protected override String GetSpecifier()
        {
            return "r";
        }
    }
}
