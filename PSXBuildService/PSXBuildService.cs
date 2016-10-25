using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PSXBuilderNetworking;
using ApplicationFramework;

namespace PSXBuildService
{
    class PSXBuildService : Application
    {
        [SettingsField(NetworkingSystem.LocalHost)]
        public String BuildMachineAddress = null;

        [SettingsField("C:\\Psyq")]
        public String SDKPath = null;

        [SettingsField("C:\\Psyq\\projects")]
        public String ProjectsPath = null;

        public NetworkingSystem NetworkingSystem = new NetworkingSystem();

        public override string GetName()
        {
            return "PSXBuildService";
        }

        public override void Initialize()
        {
            base.Initialize();

            BuildMachineAddress = NetworkingSystem.GetLocalIPAddress();
            Settings.Load();
            Settings.Save();

            NetworkingSystem.Initialize(BuildMachineAddress);
        }
    }
}
