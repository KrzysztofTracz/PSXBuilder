using System;
using System.Net.Sockets;
using System.Text;
using ApplicationFramework;

namespace PSXBuilder
{
    class PSXBuilder
    {
        public enum Settings
        {
            PSXBuildMachineAddress,
            PSXSDKPath
        }

        public static String GetValue(Settings settings)
        {
            return Properties.Settings.Default[settings.ToString()].ToString();
        }

        static int Main(String[] args)
        {
            PSXBuilderNetworking.System.Initialize();

            var application = new Application("PSXBuilder");
            var result = application.Start(args);

            return result ? 0 : -1;
        }
    }
}
