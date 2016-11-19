using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;

namespace PSXBuilder
{
    abstract class ProjectProgram : Program<PSXBuilder>
    {          
        [ProgramArgument]
        public String Project       = "\"$(MSBuildProjectFullPath)\"";

        [ProgramArgument]
        public String Configuration = "\"$(Configuration)\"";

        [ProgramArgument]
        public String Tools         = "\"$(CrtSDKReferenceVersion)\"";
    }
}
