using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using CommunicationFramework;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;
using Server = PSXBuilderNetworking.Server;

namespace PSXBuildService
{
    public class BuildSession : Session
    {
        public const String IntermediateFileExtension = "obj";
        public const String IntermediateDirectoryName = IntermediateFileExtension;
        public const String OutputDirectoryName       = "bin";

        public String IntermediateDirectory { get; protected set; }
        public String OutputDirectory       { get; protected set; }

        public List<String> FilesToCompile { get; protected set; }

        public BuildSession()
        {
            IntermediateDirectory = "";
            OutputDirectory       = "";

            FilesToCompile = null;
        }

        public override void Initialize(String user,
                                        String project,
                                        String rootDirectory,
                                        Server server,
                                        ILogger logger)
        {
            base.Initialize(user,
                            project,
                            rootDirectory,
                            server,
                            logger);

            IntermediateDirectory = Utils.Path(RootDirectory, IntermediateDirectoryName);
            OutputDirectory       = Utils.Path(RootDirectory, OutputDirectoryName);        

            PrepareDirectories();

            FilesToCompile = new List<String>();
        }

        public override void Start()
        {
            Server.SendMessage(new BuildSessionStartedMessage());
            Logger.Log("Build session started. User {0}, Project {1}", User, Project);
        }

        protected void PrepareDirectories()
        {
            CreateDirectory(RootDirectory);
            CreateDirectory(IntermediateDirectory);
            CreateDirectory(OutputDirectory);
        }

        protected void CreateDirectory(String directory)
        {
            var convertedDirectory = NamesConverter.GetShortPath(directory);
            if (!System.IO.Directory.Exists(convertedDirectory))
            {
                System.IO.Directory.CreateDirectory(convertedDirectory);
                Logger.Log("Creating directory {0}", convertedDirectory);
            }
        }

        protected override void UnsafeRegisterDelegates()
        {
            Server.RegisterDelegate<RemoveFilesMessage>(OnRemoveFilesMessage);
            Server.RegisterDelegate<ProjectFileUploadMessage>(OnProjectFileUploadMessage);
            Server.RegisterDelegate<CompilationStartMessage>(OnCompilationStartMessage);
        }

        protected override void UnsafeUnregisterDelegates()
        {
            Server.UnregisterDelegate<RemoveFilesMessage>();
            Server.UnregisterDelegate<ProjectFileUploadMessage>();
            Server.UnregisterDelegate<CompilationStartMessage>();
        }

        protected bool OnRemoveFilesMessage(RemoveFilesMessage message)
        {
            var files = message.Files;

            foreach (var file in files)
            {
                var convertedFileName = NamesConverter.GetShortPath(GetFullPath(file));
                DeleteFile(convertedFileName);
                RemoveObjFile(convertedFileName);
            }

            return true;
        }

        protected void RemoveObjFile(String file)
        {
            DeleteFile(Utils.Path(IntermediateDirectory,
                                  Utils.FileName(Utils.GetFileNameExcludingExtension(file),
                                                 IntermediateFileExtension)));
        }

        protected void DeleteFile(String fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
                Logger.Log("Deleting file {0}.", fileName);
            }
        }

        protected bool OnProjectFileUploadMessage(ProjectFileUploadMessage message)
        {
            var fileName = NamesConverter.GetShortPath(GetFullPath(message.FileName));
            Logger.Log("Writting file {0}", fileName);

            var filestream = Utils.CreateFile(fileName);
            filestream.Write(message.File, 0, message.File.Length);
            filestream.Close();

            if(message.Compile)
            {
                FilesToCompile.Add(fileName);
            }

            return true;
        }

        protected bool OnCompilationStartMessage(CompilationStartMessage message)
        {
            return true;
        }
    }
}
