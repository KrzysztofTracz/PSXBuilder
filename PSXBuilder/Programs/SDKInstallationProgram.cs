using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ApplicationFramework;
using CommunicationFramework.Messages;
using PSXBuilderNetworking.Messages;

namespace PSXBuilder.Programs
{
    class SDKInstallationProgram : Program<PSXBuilder>
    {
        public override bool Start(params String[] arguments)
        {
            var client = new PSXBuilderNetworking.Client();
            client.Inititalize(Application.NetworkingSystem.GetConnectionAddress(),
                               Application.Console);
            client.Connect();

            Log("SDK installation start.");
            client.SendMessage(new SDKInstallationStartMessage());
            client.WaitForMessage<SDKInstallationStartedMessage>();

            var files = Directory.GetFiles(Application.SDKPath,
                                           "*",
                                           SearchOption.AllDirectories);

            var checkFilesMessage = new CheckFilesMessage();

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                var fileName = Utils.ConvertPathToLocal(file,
                                                        Application.SDKPath);
                var fileSize = new FileInfo(file).Length;

                checkFilesMessage.Files.Add(fileName, fileSize);
            }

            Log("Checking installed files.");
            client.SendMessage(checkFilesMessage);
            var filesToUploadMessage = client.WaitForMessage<FileListMessage>();

            for(int i=0;i<filesToUploadMessage.Files.Count;i++)
            {
                var file = filesToUploadMessage.Files[i];

                var message = new FileUploadMessage();

                message.File     = File.ReadAllBytes(Utils.Path(Application.SDKPath, file));
                message.FileName = file;

                var installationProgress = (((float)(i + 1))/(float)filesToUploadMessage.Files.Count) * 100.0f;

                Log("[{2:000}%] Uploading file {0} ({1} bytes)", message.FileName, 
                                                                 message.File.Length,
                                                                 installationProgress);
                client.SendMessage(message);
            }

            client.SendMessage(new SDKInstallationFinishedMessage());
            Log("SDK installation finished!");

            client.Disconnect();

            return true;
        }

        protected override String[] GetArguments()
        {
            return new String[0];
        }

        protected override String GetDescription()
        {
            return "sdk installation on building machine";
        }

        protected override string GetSpecifier()
        {
            return "-i";
        }
    }
}
