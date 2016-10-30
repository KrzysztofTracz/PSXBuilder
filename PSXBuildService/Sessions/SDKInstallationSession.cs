using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using CommunicationFramework;
using CommunicationFramework.Messages;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;
using Server = PSXBuilderNetworking.Server;

namespace PSXBuildService
{
    class SDKInstallationSession : ServerSession<Server>
    {
        public String SDKPath { get; protected set; }

        public const String SDKPathToken     = "$SDKPath";
        public const String SDKBinariesPath  = "$SDKPath\\bin";
        public const String SDKLibrariesPath = "$SDKPath\\lib";
        public const String SDKIncludesPath  = "$SDKPath\\include";

        public const String SystemPathToken = "$System";
        public const String SystemTempPath  = "$System\\TEMP";

        public String[,] EnvironmentVariables = new[,]
        {
            { "PSX_PATH",            SDKBinariesPath },
            { "PSYQ_PATH",           SDKBinariesPath },
            { "COMPILER_PATH",       SDKBinariesPath },
            { "LIBRARY_PATH",        SDKLibrariesPath },
            { "C_PLUS_INCLUDE_PATH", SDKIncludesPath },
            { "C_INCLUDE_PATH",      SDKIncludesPath },
            { "GO32",                "DPMISTACK 1000000" },
            { "G032TMP",             SystemTempPath },
            { "TMPDIR",              SystemTempPath },
        };

        public void Initialize(Server server, 
                               ILogger logger,
                               String  sdkPath)
        {
            base.Initialize(server, logger);
            SDKPath = sdkPath;
        }

        public override void Start()
        {
            Server.SendMessage(new SDKInstallationStartedMessage());
            Logger.Log("Installation started.");
        }

        protected override void UnsafeRegisterDelegates()
        {   
            Server.RegisterDelegate<FileUploadMessage>(OnFileUploadMessage);
            Server.RegisterDelegate<SDKInstallationFinishedMessage>(OnSDKInstallationFinishedMessage);
            Server.RegisterDelegate<CheckFilesMessage>(OnCheckFilesMessage);
        }

        protected override void UnsafeUnregisterDelegates()
        {
            Server.UnregisterDelegate<FileUploadMessage>();
            Server.UnregisterDelegate<SDKInstallationFinishedMessage>();
            Server.UnregisterDelegate<CheckFileMessage>();
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

        protected bool OnCheckFilesMessage(CheckFilesMessage message)
        {
            Logger.Log("Checking installed files.");

            var filesToUpload = new FileListMessage();            
            foreach (var key in message.Files.Keys)
            {
                var file     = Utils.Path(SDKPath, key);
                var fileSize = message.Files[key];

                bool result = System.IO.File.Exists(file) &&
                              new System.IO.FileInfo(file).Length == fileSize;

                if(!result)
                {
                    Logger.Log("File {0} is missing.", key);
                    filesToUpload.Files.Add(key);
                }
            }

            Server.SendMessage(filesToUpload);

            return true;
        }

        protected bool OnSDKInstallationFinishedMessage(SDKInstallationFinishedMessage message)
        {
            AppendSystemPath();
            SetSystemEnvironmentVariables();
            UnregisterDelegates();
            Logger.Log("Installation finished.");

            return true;
        }

        protected void AppendSystemPath()
        {
            var sdkBinariesPath = ResolveVariable(SDKBinariesPath);
            var path = Environment.GetEnvironmentVariable("PATH", 
                                                          EnvironmentVariableTarget.Machine);

            if (!path.Contains(sdkBinariesPath))
            {
                Logger.Log("Appending system PATH with {0}.", sdkBinariesPath);

                path += sdkBinariesPath + ";";
                Environment.SetEnvironmentVariable("PATH",
                                                   path,
                                                   EnvironmentVariableTarget.Machine);
            }
        }

        protected void SetSystemEnvironmentVariables()
        {
            for(int i=0; i<EnvironmentVariables.GetLength(0); i++)
            {
                var name  = EnvironmentVariables[i, 0];
                var value = ResolveVariable(EnvironmentVariables[i, 1]);
                Logger.Log("Setting system variable {0} to {1}.", name, value);
                Environment.SetEnvironmentVariable(name, 
                                                   value,
                                                   EnvironmentVariableTarget.Machine);
            }
        }

        protected String ResolveVariable(String value)
        {
            if(value.Contains(SDKPathToken))
            {
                value = value.Replace(SDKPathToken, SDKPath);
            }

            if (value.Contains(SystemPathToken))
            {
                value = value.Replace(SystemPathToken, 
                                      Environment.GetFolderPath(Environment.SpecialFolder.Windows));
            }

            return value;
        }
    }
}
