using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class SetupProgram : ApplicationFramework.Program<PSXBuilderApplication>
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            var settings = Enum.GetValues(typeof(PSXBuilder.Settings)) as PSXBuilder.Settings[];
            if (arguments.Length == settings.Length)
            {
                for (int i = 0; i < settings.Length; i++)
                {
                    Properties.Settings.Default[settings[i].ToString()] = arguments[i];
                }
                Properties.Settings.Default.Save();
                result = true;
            }

            return result;
        }

        protected override String[] GetArguments()
        {
            var settings  = Enum.GetValues(typeof(PSXBuilder.Settings)) as PSXBuilder.Settings[];
            var arguments = new String[settings.Length];

            for (int i = 0; i < settings.Length; i++)
            {
                arguments[i] = settings[i].ToString();
            }

            return arguments;
        }

        protected override String GetDescription()
        {
            return "builder setup";
        }

        protected override String GetSpecifier()
        {
            return "-s";
        }
    }
}
