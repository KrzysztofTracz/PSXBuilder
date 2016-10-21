using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using PSXBuilderNetworking;

namespace PSXBuilder
{
    class PSXBuilderApplication : Application
    {
        public SettingsField BuildMachineAddress = new SettingsField(NetworkingSystem.LocalHost);
        public SettingsField SDKPath             = new SettingsField("..\\Psyq");

        public PSXBuilderApplication() 
            : base("PSXBuilder")
        {

        }
    }
}
