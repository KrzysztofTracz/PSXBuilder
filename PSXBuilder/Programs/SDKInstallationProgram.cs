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
        public const String SetupScript = "PSPATHS.bat";

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

            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                var fileName = Utils.ConvertPathToLocal(file,
                                                        Application.SDKPath);
                var fileSize = new FileInfo(file).Length;

                var checkFileMessage = new CheckFileMessage();
                checkFileMessage.Path = fileName;
                checkFileMessage.FileSize = fileSize;

                client.SendMessage(checkFileMessage);
                var checkFileResultMessage = client.WaitForMessage<CheckFileResultMessage>();

                if (!checkFileResultMessage.Result)
                {
                    var message = new FileUploadMessage();

                    message.File = File.ReadAllBytes(file);
                    message.FileName = Utils.ConvertPathToLocal(file,
                                                                Application.SDKPath);

                    Log("Uploading file {0} [{1}/{2}]", message.FileName, i + 1, files.Length);

                    client.SendMessage(message);
                }
            }

            client.SendMessage(new GetSDKPathMessage());
            var sdkPathMessage = client.WaitForMessage<SDKPathMessage>();

            client.SendMessage(new RunProcessMessage(Utils.Path(sdkPathMessage.Path, SetupScript), true));
            var processResultMessage = client.WaitForMessage<ProcessResultMessage>();
            Log(processResultMessage.Output);

            client.SendMessage(new SDKInstallationFinishedMessage());
            Log("SDK installation finished!");

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
