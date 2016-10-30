using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder.Programs
{
    class ExecuteProgram : ApplicationFramework.Program<PSXBuilder>
    {
        public override bool Start(params String[] arguments)
        {
            var process = new Process(Application.EPSXEPath, "-nogui", "\"" + arguments[0] + "\"");
            process.Run(Application.Console, false);

            return true;
        }

        protected override String[] GetArguments()
        {
            return new[] { "psx.exe" };
        }

        protected override String GetDescription()
        {
            return "run psx executable";
        }

        protected override String GetSpecifier()
        {
            return "-e";
        }
    }
}
