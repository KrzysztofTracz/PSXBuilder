using System;
using System.Net.Sockets;
using System.Text;
using ApplicationFramework;
using PSXBuilderNetworking;

namespace PSXBuilder
{
    class Program
    {
        static int Main(String[] args)
        {
            var application = new PSXBuilder();

            application.Initialize();

            return application.Start(args) ? 0 : -1;
        }
    }
}
