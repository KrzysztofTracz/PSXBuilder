using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class DisplaySettingsProgram : ApplicationFramework.Program
    {
        public override bool Start(params String[] arguments)
        {
            var settings = Enum.GetValues(typeof(PSXBuilder.Settings)) as PSXBuilder.Settings[];
            Application.Console.PushTab();
            for (int i = 0; i < settings.Length; i++)
            {
                var settingsName = settings[i].ToString();
                Application.Console.WriteLine("{0}: {1}",
                                              settingsName,
                                              Properties.Settings.Default[settingsName]);

            }
            Application.Console.PopTab();
            return true;
        }

        protected override String[] GetArguments()
        {
            return new String[] { };
        }

        protected override String GetDescription()
        {
            return "display builder settings";
        }

        protected override String GetSpecifier()
        {
            return "-d";
        }
    }
}
