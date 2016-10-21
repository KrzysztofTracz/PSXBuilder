using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using CommunicationFramework;
using CommunicationFramework.Messages;

namespace UploaderService
{
    class UploaderService
    {
        static NetworkingSystem NetworkingSystem = new NetworkingSystem();
        static ApplicationFramework.Console Console = new ApplicationFramework.Console();

        static void Main(string[] args)
        {
            NetworkingSystem.Initialize("14000");

            var server = new Server();
            server.Inititalize(NetworkingSystem.GetConnectionAddress(),
                               Console);

            server.RegisterDelegate<TaskKillMessage>(OnTaskKillMessage);
            server.RegisterDelegate<RunProcessMessage>(OnRunProcessMessage);
            server.RegisterDelegate<FileUploadMessage>(OnFileUploadMessage);

            server.Start();
        }

        static bool OnTaskKillMessage(TaskKillMessage message)
        {
            bool result = false;

            var process = new Process("taskkill", "/f", "/t", "/im", message.ExeName);
            result = process.Run(Console) == 0;

            return result;
        }

        static bool OnRunProcessMessage(RunProcessMessage message)
        {
            bool result = false;

            var process = new ApplicationFramework.Process(message.Process, message.Arguments);
            result = process.Run(Console, false) == 0;

            return result;
        }

        static bool OnFileUploadMessage(FileUploadMessage message)
        {
            bool result = false;

            var path     = message.FileName;
            var fileName = Utils.GetFileName(path);

            Console.Log("File {0} received [{1} bytes]",
                        fileName,
                        message.File.Length);

            var filestream = Utils.CreateFile(path);
            filestream.Write(message.File, 0, message.File.Length);
            filestream.Close();

            Console.Log("Saved at {0}",
                        path);

            return result;
        }
    }
}
