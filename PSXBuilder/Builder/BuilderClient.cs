using System;
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

        public String SDKPath { get; protected set; }

        public BuilderClient()
        {
            Project   = null;
            BuildInfo = null;
            Client    = null;
        }

        public void Initialize(PSXProject project, 
                               String buildMachineAddress, 
                               ILogger logger, 
                               String sdkPath)
        {
            Project = project;

            BuildInfo = new BuildInfo();
            BuildInfo.Initialize(logger);
            BuildInfo.Load(project.IntermediateDir);

            Client = new Client();
            Client.Inititalize(buildMachineAddress, logger);

            Logger = logger;

            SDKPath = sdkPath;
        }

        public bool Build()
        {
            bool result = false;

            List<String> filesToUpload;
            List<String> filesToRemove;
            List<String> filesToCompile;

            var user    = Environment.MachineName;
            var project = Project.Name;

            PrepareFiles(out filesToUpload, out filesToRemove, out filesToCompile);

            if (filesToUpload.Count  == 0 && 
                filesToRemove.Count  == 0 &&
                filesToCompile.Count == 0)
            {
                Logger.Log("Skipping build. No changes detected in project files. Project: {0}", project);
                return BuildInfo.Successful;
            }

            Client.Connect();
            Logger.Log("Starting build session. User: {0}, Project: {1}, Configuration: {2}", user, project, Project.Configuration);
            BuildInfo.Time = DateTime.Now;

            var buildSessionStartMessage = new BuildSessionStartMessage();
            buildSessionStartMessage.User          = user;
            buildSessionStartMessage.Project       = project;
            buildSessionStartMessage.Output        = Utils.GetFileNameExcludingExtension(Project.OutputFileName);
            buildSessionStartMessage.ProjectPath   = Project.Directory;
            buildSessionStartMessage.SDKPath       = SDKPath;
            buildSessionStartMessage.Configuration = Project.Configuration;

            Client.SendMessage(buildSessionStartMessage);
            Client.WaitForMessage<BuildSessionStartedMessage>();
            Logger.Log("Build session started.");

            Client.SendMessage(new RemoveFilesMessage(filesToRemove));

            foreach(var file in filesToUpload)
            {
                var fileUploadMessage = new ProjectFileUploadMessage();
                fileUploadMessage.FileName = file;
                fileUploadMessage.File     = System.IO.File.ReadAllBytes(Utils.Path(Project.Directory, file));

                Logger.Log("Uploading file {0} [{1} bytes].", fileUploadMessage.FileName,
                                                              fileUploadMessage.File.Length);
                Client.SendMessage(fileUploadMessage);
            }

            bool startLinker      = false;
            bool createExecutable = false;
            bool downloadBinaries = false;

            Logger.Log("");

            startLinker = StartRemoteProcess<CompilationResultMessage>
                                            (new CompilationStartMessage(filesToCompile, 
                                                                         Project.PreprocessorDefinitions),
                                            "Starting compilation.");

            if (startLinker)
            {
                createExecutable = StartRemoteProcess<LinkingProcessStartMessage, 
                                                      LinkingProcessResultMessage>
                                                     ("Starting linker.");
            }

            if(createExecutable)
            {
                downloadBinaries = StartRemoteProcess<CreatingExecutableResultMessage>
                                                     (new CreateExecutableMessage(Project.VideoFormat),
                                                     "Creating executable.");
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

            BuildInfo.Successful = result;
            SaveBuildInfo();

            return result;
        }

        protected bool StartRemoteProcess<RESULT>(CommunicationFramework.Message message,
                                                  String                         log)
            where RESULT : ProcessResultMessage
        {
            Logger.Log(log);
            Client.SendMessage(message);
            var resultMessage = Client.WaitForMessage<RESULT>();
            Logger.Log(resultMessage.Output);
            return resultMessage.ReturnCode == 0;
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

        protected void CheckFile(PSXProject.File file, out bool modified, out bool dependencyModified, List<String> cycleGuard = null)
        {
            modified           = false;
            dependencyModified = false;

            if (cycleGuard == null)
            {
                cycleGuard = new List<String>();
            }

            if (!cycleGuard.Contains(file.LocalPath))
            {
                if (!BuildInfo.Files.Contains(file.LocalPath))
                {
                    modified = true;
                }
                else if (file.LastModified > BuildInfo.Time)
                {
                    modified = true;
                }
                else
                {
                    foreach (var dependency in file.Dependencies)
                    {
                        bool m = false;
                        bool d = false;

                        cycleGuard.Add(file.LocalPath);

                        CheckFile(Project.GetFile(Utils.TrimPath(Utils.Path(Utils.GetDirectory(file.LocalPath), dependency))), 
                                  out m, 
                                  out d, 
                                  cycleGuard);

                        if (m || d)
                        {
                            dependencyModified = true;
                        }
                    }
                }
            }
        }

        protected void PrepareFiles(out List<String> filesToUpload, out List<String> filesToRemove, out List<String> filesToCompile)
        {
            filesToUpload  = new List<String>();
            filesToRemove  = new List<String>();
            filesToCompile = new List<String>();

            foreach (var file in Project.Files)
            {
                bool pushFile    = false;
                bool compileFile = false;

                CheckFile(file, out pushFile, out compileFile);

                if (pushFile)
                {
                    filesToUpload.Add(file.LocalPath);
                }

                if(file.Type == PSXProject.FileType.Source && (pushFile || compileFile))
                {
                    filesToCompile.Add(file.LocalPath);
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
            BuildInfo.Save(Project.IntermediateDir);
        }
    }
}
