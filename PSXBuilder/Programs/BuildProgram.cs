using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class BuildProgram : ApplicationFramework.Program<PSXBuilder>
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            var project = new PSXProject();
            if (project.Load(arguments[0], arguments[1], arguments[2]))
            {
                var builder = new Builder();
                builder.Initialize(project, 
                                   Application.NetworkingSystem.GetConnectionAddress(), 
                                   Application.Console);
                builder.Build();

                result = true;
            }

            return result;
        }

        protected override String[] GetArguments()
        {
            return new[] { "projectFile", "configuration", "toolsVersion" };
        }

        protected override String GetDescription()
        {
            return "build project";
        }

        protected override String GetSpecifier()
        {
            return "-b";
        }
    }
}
