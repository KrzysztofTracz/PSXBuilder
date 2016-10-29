using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;

namespace PSXBuilder
{
    class Builder
    {
        public PSXProject Project   { get; protected set; }
        public BuildInfo  BuildInfo { get; protected set; }

        public Client Client { get; protected set; }

        public Builder()
        {
            Project   = null;
            BuildInfo = null;
            Client    = null;
        }

        public void Initialize(PSXProject project, String buildMachineAddress, ILogger logger)
        {
            Project = project;

            BuildInfo = new BuildInfo();
            BuildInfo.Load(project.IntermediateDir);

            Client = new Client();
            Client.Inititalize(buildMachineAddress, logger);
        }

        public bool Build()
        {
            bool result = false;

            List<PSXProject.File> filesToUpload;
            List<String>          filesToRemove;

            PrepareFiles(out filesToUpload, out filesToRemove);
            SaveBuildInfo();

            Client.Connect();
            Client.SendMessage(new BuildSessionStartMessage(Environment.MachineName, Project.Name));
            Client.SendMessage(new RemoveFilesMessage(filesToRemove));

            foreach(var file in filesToUpload)
            {
                var fileUploadMessage = new ProjectFileUploadMessage();
                fileUploadMessage.Compile  = file.Type == PSXProject.FileType.Source;
                fileUploadMessage.FileName = file.LocalPath;
                fileUploadMessage.File     = System.IO.File.ReadAllBytes(file.Path);

                Client.SendMessage(fileUploadMessage);
            }

            Client.SendMessage(new CompilationStartMessage());
            var compilationResult = Client.WaitForMessage<CompilationResultMessage>();

            return result;
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
