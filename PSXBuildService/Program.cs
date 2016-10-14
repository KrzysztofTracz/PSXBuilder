using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PSXBuildService
{
    class Program
    {
        static void Main(string[] args)
        {
            PSXBuilderNetworking.System.Initialize();

            var server = new PSXBuilderNetworking.Server();
            server.Inititalize("127.0.0.1:13000");
            server.Start();
        }
    }
}
