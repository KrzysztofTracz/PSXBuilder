﻿using System;
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

        [SettingsField("..\\ePSXe\\ePSXe.exe")]
        public String EPSXEPath = null;

        [SettingsField("..\\ePSXe\\bios\\SCPH1001.BIN")]
        public String EPSXEBios = null;

        public NetworkingSystem NetworkingSystem = new NetworkingSystem();

        public override String GetName()
        {
            return "PSXBuilder";
        }

        public override void Initialize()
        {
            base.Initialize();
            NetworkingSystem.Initialize(BuildMachineAddress);
            Settings.Save();
        }
    }
}
