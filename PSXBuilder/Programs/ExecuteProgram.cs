using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder.Programs
{
    class ExecuteProgram : ApplicationFramework.Program<PSXBuilder>
    {
        [ProgramArgument]
        public String Exe = null;

        public override bool Start()
        {
            var process = new Process(Utils.Quotes(Application.EPSXEPath), 
                                      "-nogui", 
                                      "-bios", 
                                      Utils.Quotes(Application.EPSXEBios), Utils.Quotes(Exe));

            process.Run(Application.Console, false);

            return true;
        }

        protected override String GetDescription()
        {
            return "Run psx executable";
        }

        protected override String GetSpecifier()
        {
            return "e";
        }
    }
}
