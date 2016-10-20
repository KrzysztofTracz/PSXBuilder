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
        const char Separator = '\\';

        static NetworkingSystem NetworkingSystem = new NetworkingSystem();

        static int Main(string[] args)
        {
            int result = -1; 
            if (args.Length == 3)
            {
                var ip              = args[0];
                var path            = args[1];
                var targetDirectory = args[2];

                var filename  = GetFileName(path);
                var directory = path.Replace(filename, "");
                
                if (directory.EndsWith(Separator.ToString()))
                {
                    directory = directory.Substring(0, directory.Length - 1);
                }

                NetworkingSystem.Initialize("14000", ip);

                var client = new Client();
                client.Inititalize(NetworkingSystem.GetConnectionAddress(),
                                   new ApplicationFramework.Console());
                client.Connect();

                SendTaskKill(client, filename);
                SendFile(client, path, targetDirectory);

                var dlls = System.IO.Directory.EnumerateFiles(directory, "*.dll");
                foreach(var dll in dlls)
                {
                    SendFile(client, dll, targetDirectory);
                }

                SendRunProcess(client, targetDirectory + Separator + filename);

                result = 0;
            }
            return result;
        }

        static void SendTaskKill(Client client, String exeName)
        {
            var message = new TaskKillMessage();
            message.ExeName = exeName;

            client.SendMessage(message);
        }

        static void SendFile(Client client, String path, String targetDirectory)
        {
            var message = new FileUploadMessage();
            message.FileName = targetDirectory + Separator + GetFileName(path);
            message.File     = System.IO.File.ReadAllBytes(path);

            Console.WriteLine("Uploading {0} [{1} bytes]",
                              path,
                              message.File.Length);

            client.SendMessage(message);
        }

        static void SendRunProcess(Client client, String process, params String[] arguments)
        {
            client.SendMessage(new RunProcessMessage(process, arguments));
        }

        static String GetFileName(String path)
        {
            return path.Split(Separator).Last();
        }
    }
}
