﻿using PSXBuilderNetworking;
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
            Server.RegisterDelegate<GetSDKPathMessage>(OnGetSDKPathMessage);
            Server.RegisterDelegate<RunProcessMessage>(OnRunProcessMessage);

            try
            {
                Server.Start();
            }
            catch (Exception e)
            {
                while (e != null)
                {
                    Log(e.Message);
                    Log(e.StackTrace);
                    e = e.InnerException;
                }
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
            var sdkInstallator = new SDKInstallator();
            sdkInstallator.Initialize(Server,
                                      Application.Console,
                                      Application.SDKPath);

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
