using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilder
{
    class DisplaySettingsProgram : Program<PSXBuilder>
    {
        public override bool Start(params String[] arguments)
        {
            Application.Console.PushTab();
            for (int i = 0; i < Application.Settings.Count; i++)
            {
                Application.Console.WriteLine("{0}: {1}",
                                              Application.Settings.GetName(i),
                                              Application.Settings.GetValue(i));

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
