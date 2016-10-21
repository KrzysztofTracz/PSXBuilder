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
    class PSXBuildService
    {
        static NetworkingSystem NetworkingSystem    = new NetworkingSystem();
        static ApplicationFramework.Console Console = new ApplicationFramework.Console();

        static void Main(string[] args)
        {
            NetworkingSystem.Initialize();

            var server = new Server();
            server.Inititalize(NetworkingSystem.GetConnectionAddress(),
                               Console);
            server.Start();
        }
    }
}
