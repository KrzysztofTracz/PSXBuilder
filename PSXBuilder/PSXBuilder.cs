using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using PSXBuilderNetworking;

namespace PSXBuilder
{
    class PSXBuilder : Application
    {
        [SettingsField(NetworkingSystem.LocalHost)]
        public String BuildMachineAddress = null;

        [SettingsField("..\\Psyq")]
        public String SDKPath = null;

        public NetworkingSystem NetworkingSystem = new NetworkingSystem();

        public override string GetName()
        {
            return "PSXBuilder";
        }

        public override void Initialize()
        {
            base.Initialize();
            InitializePrograms();
            NetworkingSystem.Initialize(BuildMachineAddress);
        }
    }
}
