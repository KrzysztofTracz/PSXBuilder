using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace Uploader
{
    class Uploader
    {
        static NetworkingSystem NetworkingSystem = new NetworkingSystem();

        static int Main(string[] args)
        {
            int result = -1; 
            if (args.Length == 2)
            {
                var ip       = NetworkingSystem.LocalHost;
                var path     = args[1];
                var filename = path.Split('\\').Last();
                var file     = System.IO.File.ReadAllBytes(path);

                NetworkingSystem.Initialize("14000", ip);

                var client = new Client();
                client.Inititalize(NetworkingSystem.GetConnectionAddress(),
                                   new DefaultDeviceLog());
                client.Connect();

                var message = new CommunicationFramework.Messages.FileUploadMessage();
                message.FileName = filename;
                message.File     = file;

                client.SendMessage(message);

                result = 0;
            }
            return result;
        }
    }
}
