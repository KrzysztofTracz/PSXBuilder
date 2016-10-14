using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXBuilderNetworking;

namespace PSXBuilder
{
    class ConnectionTestProgram : ApplicationFramework.Program
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            var buildMachineAddress = "127.0.0.1:13000";//PSXBuilder.GetValue(PSXBuilder.Settings.PSXBuildMachineAddress);

            var client = new Client();                        
            client.Inititalize(buildMachineAddress);
            client.Connect();
            Application.Console.WriteLine("Pinging build machine at {0}...", buildMachineAddress);
            result = client.Ping();
            Application.Console.WriteLine(result ? "Success!" : "Fail!");
            client.Disconnect();

            return result;
        }

        protected override String[] GetArguments()
        {
            return new String[] { };
        }

        protected override String GetDescription()
        {
            return "build machine connection test";
        }

        protected override String GetSpecifier()
        {
            return "-t";
        }
    }
}
