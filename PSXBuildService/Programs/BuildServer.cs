using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework.Messages;

namespace PSXBuildService.Programs
{
    class BuildServer : ApplicationFramework.Program<PSXBuildService>
    {
        public Server Server { get; protected set; }

        public override bool Start(params String[] arguments)
        {
            bool result = true;

            Server = new Server();
            Server.Inititalize(Application.NetworkingSystem.GetConnectionAddress(),
                               Application.Console);

            Server.RegisterDelegate<SDKInstallationStartMessage>(OnSDKInstallationStartMessage);
            Server.RegisterDelegate<BuildSessionStartMessage>(OnBuildSessionStartMessage);
            Server.RegisterDelegate<CleanSessionStartMessage>(OnCleanSessionStartMessage);

            Server.RegisterDelegate<GetSDKPathMessage>(OnGetSDKPathMessage);
            Server.RegisterDelegate<RunProcessMessage>(OnRunProcessMessage);

            try
            {
                Server.Start();
            }
            catch (Exception e)
            {
                Application.Console.Log(e);
                result = false;
            }

            return result;
        }

        protected override String[] GetArguments()
        {
            return new String[0];
        }

        protected override String GetDescription()
        {
            return "building server";
        }

        protected override String GetSpecifier()
        {
            return "";
        }

        protected bool OnSDKInstallationStartMessage(SDKInstallationStartMessage message)
        {
            var sdkInstallator = new SDKInstallationSession();
            sdkInstallator.Initialize(Server,
                                      Application.Console,
                                      Application.SDKPath);
            sdkInstallator.Start();
            return true;
        }

        protected bool OnBuildSessionStartMessage(BuildSessionStartMessage message)
        {
            var builder = new BuildSession();
            builder.Initialize(message.User,
                               message.Project,
                               Application.ProjectsPath,
                               message.Output,
                               Server,
                               Application.Console);
            builder.Start();
            return true;
        }

        protected bool OnCleanSessionStartMessage(CleanSessionStartMessage message)
        {
            var cleaner = new CleanSession();
            cleaner.Initialize(message.User,
                               message.Project,
                               Application.ProjectsPath,
                               Server,
                               Application.Console);
            cleaner.Start();
            return true;
        }

        protected bool OnGetSDKPathMessage(GetSDKPathMessage message)
        {
            var sdkPathMessage = new SDKPathMessage();
            sdkPathMessage.Path = Application.SDKPath;
            Server.SendMessage(sdkPathMessage);

            return true;
        }

        protected bool OnRunProcessMessage(RunProcessMessage message)
        {
            var process    = new ApplicationFramework.Process(message.Process, message.Arguments);
            var returnCode = process.Run(Application.Console, message.WaitForExit);

            if(message.WaitForExit)
            {
                var processResultMessage = new ProcessResultMessage();
                processResultMessage.ReturnCode = returnCode;
                processResultMessage.Output     = process.Output;

                Server.SendMessage(processResultMessage);
            }

            return returnCode == 0;
        }
    }
}
