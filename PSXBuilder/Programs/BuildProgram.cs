using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;

namespace PSXBuilder
{
    class BuildProgram : ProjectProgram
    {
        public override bool Start()
        {
            bool result = false;

            var project = new PSXProject();
            if (project.Load(Project, Configuration, Tools))
            {
                var builder = new BuilderClient();
                builder.Initialize(project, 
                                   Application.NetworkingSystem.GetConnectionAddress(), 
                                   Application.Console,
                                   Application.SDKPath);

                result = builder.Build();
            }

            return result;
        }

        protected override String GetDescription()
        {
            return "Build project";
        }

        protected override String GetSpecifier()
        {
            return "b";
        }
    }
}
