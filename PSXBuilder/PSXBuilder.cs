using System;
using System.Net.Sockets;
using System.Text;
using ApplicationFramework;
using PSXBuilderNetworking;

namespace PSXBuilder
{
    class PSXBuilder
    {
        public enum Settings
        {
            PSXBuildMachineAddress,
            PSXSDKPath
        }

        public static NetworkingSystem NetworkingSystem = new NetworkingSystem();

        public static String GetValue(Settings settings)
        {
            return Properties.Settings.Default[settings.ToString()].ToString();
        }

        static int Main(String[] args)
        {
            NetworkingSystem.Initialize(GetValue(Settings.PSXBuildMachineAddress));

            var application = new Application("PSXBuilder");
            var result = application.Start(args);

            return result ? 0 : -1;
        }
    }
}
