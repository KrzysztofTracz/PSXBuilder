using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplicationFramework
{
    class DisplaySettingsProgram : Program<Application>
    {
        public override bool Start()
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

        protected override String GetDescription()
        {
            return "Display settings";
        }

        protected override String GetSpecifier()
        {
            return "d";
        }
    }
}
