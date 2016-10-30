﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using CommunicationFramework.Messages;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;

namespace PSXBuilder
{
    class BuilderClient
    {
        public PSXProject Project   { get; protected set; }
        public BuildInfo  BuildInfo { get; protected set; }

        public Client  Client { get; protected set; }
        public ILogger Logger { get; protected set; }

        public BuilderClient()
        {
            Project   = null;
            BuildInfo = null;
            Client    = null;
        }

        public void Initialize(PSXProject project, String buildMachineAddress, ILogger logger)
        {
            Project = project;

            BuildInfo = new BuildInfo();
            BuildInfo.Initialize(logger);
            BuildInfo.Load(project.IntermediateDir);

            Client = new Client();
            Client.Inititalize(buildMachineAddress, logger);

            Logger = logger;
        }

        public bool Build()
        {
            bool result = false;

            List<PSXProject.File> filesToUpload;
            List<String>          filesToRemove;

            Client.Connect();

            var user    = Environment.MachineName;
            var project = Project.Name;

            Logger.Log("Starting build session. User: {0}, Project: {1}.", user, project);
            var buildSessionStartMessage = new BuildSessionStartMessage();
            buildSessionStartMessage.User    = user;
            buildSessionStartMessage.Project = project;
            buildSessionStartMessage.Output  = Utils.GetFileNameExcludingExtension(Project.OutputFileName);

            Client.SendMessage(buildSessionStartMessage);
            Client.WaitForMessage<BuildSessionStartedMessage>();
            Logger.Log("Build session started.");

            PrepareFiles(out filesToUpload, out filesToRemove);
            SaveBuildInfo();

            Client.SendMessage(new RemoveFilesMessage(filesToRemove));

            foreach(var file in filesToUpload)
            {
                var fileUploadMessage = new ProjectFileUploadMessage();
                fileUploadMessage.Compile  = file.Type == PSXProject.FileType.Source;
                fileUploadMessage.FileName = file.LocalPath;
                fileUploadMessage.File     = System.IO.File.ReadAllBytes(file.Path);

                Logger.Log("Uploading file {0} [{1} bytes].", fileUploadMessage.FileName,
                                                              fileUploadMessage.File.Length);
                Client.SendMessage(fileUploadMessage);
            }

            bool startLinker      = false;
            bool createExecutable = false;
            bool downloadBinaries = false;

            startLinker = StartRemoteProcess<CompilationStartMessage, 
                                             CompilationResultMessage>
                                            ("Starting compilation.");
            if (startLinker)
            {
                createExecutable = StartRemoteProcess<LinkingProcessStartMessage, 
                                                      LinkingProcessResultMessage>
                                                     ("Starting linker.");
            }

            if(createExecutable)
            {
                downloadBinaries = StartRemoteProcess<CreateExecutableMessage, 
                                                      CreatingExecutableResultMessage>
                                                     ("Creating executable.");
            }

            if (downloadBinaries)
            {
                Logger.Log("Downloading binaries.");
                Client.RegisterDelegate<FileUploadMessage>(OnFileUploadMessage);
                Client.SendMessage(new DownloadProjectBinariesMessage());
                Client.WaitForMessage<ProjectBinariesDownloadedMessage>();
                Client.UnregisterDelegate<FileUploadMessage>();
                Logger.Log("Binaries downloaded.");
                result = true;
            }

            return result;
        }

        protected bool StartRemoteProcess<INVOKE, RESULT>(String log) where INVOKE : EmptyMessage, new()
                                                                      where RESULT : ProcessResultMessage
        {
            Logger.Log(log);
            Client.SendMessage(new INVOKE());
            var resultMessage = Client.WaitForMessage<RESULT>();
            Logger.Log(resultMessage.Output);
            return resultMessage.ReturnCode == 0;
        }

        protected bool OnFileUploadMessage(FileUploadMessage message)
        {
            var fileName = Utils.Path(Project.OutputDir, message.FileName);    
            Logger.Log("Writting file {0}", Utils.GetFileName(fileName));

            var filestream = Utils.CreateFile(fileName);
            filestream.Write(message.File, 0, message.File.Length);
            filestream.Close();

            return true;
        }

        protected void PrepareFiles(out List<PSXProject.File> filesToUpload, out List<String> filesToRemove)
        {
            filesToUpload = new List<PSXProject.File>();
            filesToRemove = new List<String>();

            foreach (var file in Project.Files)
            {
                bool pushFile = false;
                if (!BuildInfo.Files.Contains(file.LocalPath))
                {
                    pushFile = true;
                }
                else if (file.LastModified > BuildInfo.Time)
                {
                    pushFile = true;
                }

                if (pushFile)
                {
                    filesToUpload.Add(file);
                }
            }

            foreach (var file in BuildInfo.Files)
            {
                if (!Project.Contains(file))
                {
                    filesToRemove.Add(file);
                }
            }

            BuildInfo.Files.Clear();
            foreach (var file in Project.Files)
            {
                BuildInfo.Files.Add(file.LocalPath);
            }
        }

        protected void SaveBuildInfo()
        {
            BuildInfo.Time = DateTime.Now;
            BuildInfo.Save(Project.IntermediateDir);
        }
    }
}
