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

            var filename  = message.FileName;
            var separator = '\\';
            var split     = filename.Split(separator);

            if (split.Length > 1)
            {
                var dir = new StringBuilder();
                for(int i=0;i<split.Length - 1;i++)
                {
                    var d = split[i];
                    bool createDirectory = false;
                    if(d != "." && d != "..")
                    {
                        createDirectory = true;
                    }
                    dir.Append(d);

                    if (createDirectory)
                    {
                        var dirStr = dir.ToString();
                        if (!System.IO.Directory.Exists(dirStr))
                        {
                            System.IO.Directory.CreateDirectory(dirStr);
                        }
                    }

                    dir.Append(separator);
                }
            }

            Console.WriteLine("File {0} received [{1} bytes]",
                              split.Last(),
                              message.File.Length);

            var filestream = System.IO.File.Create(filename);
            filestream.Write(message.File, 0, message.File.Length);
            filestream.Close();

            Console.WriteLine("Saved at {0}",
                              filename);

            return result;
        }
    }
}
