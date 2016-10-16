using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PSXBuilderNetworking;

namespace PSXBuildService
{
    class PSXBuildService
    {
        static NetworkingSystem NetworkingSystem = new NetworkingSystem();

        static void Main(string[] args)
        {
            NetworkingSystem.Initialize();

            var deviceListener = new DeviceListener();

            var server = new Server();
            server.Inititalize(NetworkingSystem.GetConnectionAddress(), 
                               deviceListener);
            server.Start();
        }

        class DeviceListener : CommunicationFramework.IDeviceListener
        {
            public void WriteLine(string text)
            {
                Console.WriteLine(">> " + text);
            }
        }
    }
}
