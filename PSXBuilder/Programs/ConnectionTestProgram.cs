﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXBuilderNetworking;

namespace PSXBuilder
{
    class ConnectionTestProgram : ApplicationFramework.Program, CommunicationFramework.IDeviceListener
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            var client = new Client();                        
            client.Inititalize(PSXBuilder.NetworkingSystem.GetConnectionAddress(),
                               this);
            client.Connect();
            Application.Console.WriteLine("Pinging build machine at {0}...", PSXBuilder.NetworkingSystem.GetConnectionAddress());
            result = client.Ping();
            Application.Console.WriteLine(result ? "Success!" : "Fail!");
            client.Disconnect();

            return result;
        }

        public void WriteLine(string text)
        {
            Application.Console.WriteLine(">> " + text);
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