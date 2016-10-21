using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    class SetupProgram : Program<Application>
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            if (arguments.Length == Application.Settings.Count)
            {
                for (int i = 0; i < Application.Settings.Count; i++)
                {
                    Application.Settings[i] = arguments[i];
                }
                Application.Settings.Save();
                result = true;
            }

            return result;
        }

        protected override String[] GetArguments()
        {
            var arguments = new String[Application.Settings.Count];

            for (int i = 0; i < Application.Settings.Count; i++)
            {
                arguments[i] = Application.Settings.GetName(i);
            }

            return arguments;
        }

        protected override String GetDescription()
        {
            return "setup settings";
        }

        protected override String GetSpecifier()
        {
            return "-s";
        }
    }
}
