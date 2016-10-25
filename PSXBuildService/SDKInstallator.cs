using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using CommunicationFramework.Messages;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;

namespace PSXBuildService
{
    class SDKInstallator
    {
        public Server  Server  { get; protected set; }
        public ILogger Logger  { get; protected set; }
        public String  SDKPath { get; protected set; }

        public void Initialize(Server  server, 
                               ILogger logger,
                               String  sdkPath)
        {
            Server  = server;
            Logger  = logger;
            SDKPath = sdkPath;

            Server.RegisterDelegate<FileUploadMessage>(OnFileUploadMessage);
            Server.RegisterDelegate<SDKInstallationFinishedMessage>(OnSDKInstallationFinishedMessage);
            Server.RegisterDelegate<CheckFileMessage>(OnCheckFileMessage);

            Server.SendMessage(new SDKInstallationStartedMessage());
            Logger.Log("Installation started.");
        }

        protected bool OnFileUploadMessage(FileUploadMessage message)
        {
            var file = Utils.Path(SDKPath, message.FileName);
            Logger.Log("Installing {0}", file);

            var filestream = Utils.CreateFile(file);
            filestream.Write(message.File, 0, message.File.Length);
            filestream.Close();

            return true;
        }

        protected bool OnCheckFileMessage(CheckFileMessage message)
        {
            var file     = Utils.Path(SDKPath, message.Path);
            var fileSize = message.FileSize;

            Logger.Log("Checking {0}", file);

            bool result = System.IO.File.Exists(file) &&
                          new System.IO.FileInfo(file).Length == fileSize;

            Server.SendMessage(new CheckFileResultMessage(result));

            return true;
        }

        protected bool OnSDKInstallationFinishedMessage(SDKInstallationFinishedMessage message)
        {
            Logger.Log("Installation finished.");
            Server.UnregisterDelegate<FileUploadMessage>();
            Server.UnregisterDelegate<SDKInstallationFinishedMessage>();
            Server.UnregisterDelegate<CheckFileMessage>();

            return true;
        }
    }
}
