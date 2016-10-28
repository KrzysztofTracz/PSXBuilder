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

        static Server Server = null;

        static void Main(string[] args)
        {
            NetworkingSystem.Initialize("14000");

            Server = new Server();
            Server.Inititalize(NetworkingSystem.GetConnectionAddress(),
                               Console);

            Server.RegisterDelegate<TaskKillMessage>(OnTaskKillMessage);
            Server.RegisterDelegate<RunProcessMessage>(OnRunProcessMessage);
            Server.RegisterDelegate<FileUploadMessage>(OnFileUploadMessage);

            try
            {
                Server.Start();
            }
            catch(Exception e)
            {
                while(e != null)
                {
                    Console.Log(e.Message);
                    Console.Log(e.StackTrace);
                    e = e.InnerException;
                }
            }
        }

        static bool OnTaskKillMessage(TaskKillMessage message)
        {
            bool result = false;

            var process = new Process("taskkill", "/f", "/t", "/im", message.ExeName);
            result = process.Run(Console) == 0;

            Server.SendMessage(new TaskKilledMessage());

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
