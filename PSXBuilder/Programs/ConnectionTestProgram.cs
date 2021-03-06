﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXBuilderNetworking;
using ApplicationFramework;

namespace PSXBuilder
{
    class ConnectionTestProgram : Program<PSXBuilder>
    {
        public override bool Start()
        {
            bool result = false;

            var client = new Client();                        
            client.Inititalize(Application.NetworkingSystem.GetConnectionAddress(),
                               Application.Console);
            client.Connect();
            Log("Pinging build machine at {0}...", Application.NetworkingSystem.GetConnectionAddress());
            result = client.Ping();
            Log(result ? "Success!" : "Fail!");
            client.Disconnect();

            return result;
        }

        protected override String GetDescription()
        {
            return "Build machine connection test";
        }

        protected override String GetSpecifier()
        {
            return "t";
        }
    }
}
