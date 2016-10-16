using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;
using CommunicationFramework.Messages;

namespace UploaderService
{
    class UploaderService
    {
        static NetworkingSystem NetworkingSystem = new NetworkingSystem();

        static void Main(string[] args)
        {
            NetworkingSystem.Initialize("14000", NetworkingSystem.LocalHost);

            var server = new Server();
            server.Inititalize(NetworkingSystem.GetConnectionAddress(),
                               new DefaultDeviceLog());
            server.RegisterDelegate<FileUploadMessage>(OnFileUploadMessage);
            server.Start();
        }

        static bool OnFileUploadMessage(FileUploadMessage message)
        {
            bool result = false;

            var filestream = System.IO.File.Create(message.FileName);
            filestream.Write(message.File, 0, message.File.Length);
            filestream.Close();

            return result;
        }
    }
}
