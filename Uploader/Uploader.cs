using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;
using CommunicationFramework.Messages;

namespace Uploader
{
    class Uploader
    {
        static NetworkingSystem NetworkingSystem = new NetworkingSystem();

        static int Main(string[] args)
        {
            int result = -1; 
            if (args.Length == 3)
            {
                var separator       = '\\';
                var ip              = args[0];
                var path            = args[1];
                var filename        = path.Split(separator).Last();
                var file            = System.IO.File.ReadAllBytes(path);
                var targetDirectory = args[2];

                NetworkingSystem.Initialize("14000", ip);

                var client = new Client();
                client.Inititalize(NetworkingSystem.GetConnectionAddress(),
                                   new DefaultDeviceLog());
                client.Connect();

                var message = new FileUploadMessage();
                message.FileName = targetDirectory + separator + filename;
                message.File     = file;

                Console.WriteLine("Uploading {0} [{1} bytes]",
                                  path,
                                  message.File.Length);

                client.SendMessage(message);

                result = 0;
            }
            return result;
        }
    }
}
