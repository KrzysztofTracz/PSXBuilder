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
    public class BuildSession : Session
    {
        public const String IntermediateFileExtension = "obj";
        public const String IntermediateDirectoryName = IntermediateFileExtension;
        public const String OutputDirectoryName       = "bin";
        public const String ControlFileExtension      = "cf";
        public const String BinaryFileExtension       = "cpe";
        public const String SymbolFileExtension       = "sym";
        public const String MapFileExtension          = "map";
        public const String ExecutableFileExtension   = "exe";

        public String IntermediateDirectory { get; protected set; }
        public String OutputDirectory       { get; protected set; }
        public String OutputFileName        { get; protected set; }

        public List<String> FilesToCompile { get; protected set; }

        public BuildMessageConverter MessageConverter { get; protected set; }
        public CompilationInfo       CompilationInfo  { get; protected set; }

        public BuildSession()
        {
            IntermediateDirectory = "";
            OutputDirectory       = "";
            OutputFileName        = "";

            FilesToCompile = null;
        }

        public void Initialize(String user,
                               String project,
                               String rootDirectory,
                               String sdkDirectory,
                               String originalRootDirectory,
                               String originalSDKDirectory,
                               String output,
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
            OutputFileName        = Utils.Path(OutputDirectory, output);

            PrepareDirectories();

            FilesToCompile = new List<String>();

            MessageConverter = new BuildMessageConverter(RootDirectory, originalRootDirectory,
                                                         sdkDirectory,  originalSDKDirectory,
                                                         NamesConverter);

            CompilationInfo = new CompilationInfo();
            CompilationInfo.Initialize(Project,
                                       NamesConverter.GetShortPath(RootDirectory),
                                       Logger);
        }

        public override void Start()
        {
            CompilationInfo.Load();
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
            Server.RegisterDelegate<LinkingProcessStartMessage>(OnLinkingProcessStartMessage);
            Server.RegisterDelegate<CreateExecutableMessage>(OnCreateExecutableMessage);
            Server.RegisterDelegate<DownloadProjectBinariesMessage>(OnDownloadProjectBinariesMessage);
        }

        protected override void UnsafeUnregisterDelegates()
        {
            Server.UnregisterDelegate<RemoveFilesMessage>();
            Server.UnregisterDelegate<ProjectFileUploadMessage>();
            Server.UnregisterDelegate<CompilationStartMessage>();
            Server.UnregisterDelegate<LinkingProcessStartMessage>();
            Server.UnregisterDelegate<CreateExecutableMessage>();
            Server.UnregisterDelegate<DownloadProjectBinariesMessage>();
        }

        protected bool OnRemoveFilesMessage(RemoveFilesMessage message)
        {
            var files = message.Files;

            foreach (var file in files)
            {
                var convertedFileName = NamesConverter.GetShortPath(GetFullPath(file));
                CompilationInfo.Remove(convertedFileName);
                DeleteFile(convertedFileName);
                RemoveObjFile(convertedFileName);
            }

            return true;
        }

        protected void RemoveObjFile(String file)
        {
            DeleteFile(GetObjFile(file));
        }

        protected String GetObjFile(String file)
        {
            return NamesConverter.GetShortPath(Utils.Path(IntermediateDirectory,
                                                          Utils.ConvertPathToLocal(Utils.GetDirectory(file), 
                                                                                   NamesConverter.GetShortPath(RootDirectory)),
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
                CompilationInfo.Add(fileName);
            }

            return true;
        }

        protected bool OnCompilationStartMessage(CompilationStartMessage message)
        {
            var returnCode = 0;
            var outputBuffer = new StringBuilder();
            foreach (var file in CompilationInfo.Files)
            {
                var objFile = GetObjFile(file);
                Utils.CreateDirectory(Utils.GetDirectory(objFile));

                Server.SendLog("Compiling file: {0}", Utils.GetFileName(file));               
                var process = new Process("ccpsx.exe", "-c", file, "-o", objFile);
                returnCode = process.Run(Logger);                
                if (returnCode != 0)
                {
                    outputBuffer.AppendLine(MessageConverter.ConvertMessage(process.Output));                    
                    break;
                }
                CompilationInfo.MarkAsCompiled(file);
            }

            CompilationInfo.FlushCompiledFiles();

            var resultMessage = new CompilationResultMessage();
            resultMessage.ReturnCode = returnCode;
            resultMessage.Output     = outputBuffer.ToString();

            Server.SendMessage(resultMessage);
            return true;
        }

        protected bool OnLinkingProcessStartMessage(LinkingProcessStartMessage message)
        {
            var returnCode = 0;
            var outputBuffer = new StringBuilder();

            var files = System.IO.Directory.GetFiles(NamesConverter.GetShortPath(IntermediateDirectory), 
                                                     Utils.FileName("*", IntermediateFileExtension), 
                                                     System.IO.SearchOption.AllDirectories);

            var controlFile = CreateControlFile(NamesConverter.GetShortName(Project), 
                                                files);

            for(int i=0;i<files.Length;i++)
            {
                files[i] = Utils.GetFileName(files[i]);
            }

            Server.SendLog("Linking files: {0}", Utils.ConcatArguments(", ", files));
            var outputFileName = NamesConverter.GetShortPath(OutputFileName);
            var outputFiles = Utils.ConcatArguments(",", Utils.FileName(outputFileName, BinaryFileExtension),
                                                         Utils.FileName(outputFileName, SymbolFileExtension),
                                                         Utils.FileName(outputFileName, MapFileExtension));

            var process = new Process("ccpsx.exe", "-Xo$80010000", controlFile, "-o", outputFiles);
            returnCode = process.Run(Logger);
            if (returnCode != 0)
            {
                outputBuffer.Append("LINKER : error : ");
                outputBuffer.AppendLine(process.Output);
            }

            var resultMessage = new LinkingProcessResultMessage();
            resultMessage.ReturnCode = returnCode;
            resultMessage.Output     = outputBuffer.ToString();

            Server.SendMessage(resultMessage);
            return true;
        }

        protected String CreateControlFile(String name, params String[] files)
        {
            var buffer = new StringBuilder();
            foreach(var file in files)
            {
                buffer.AppendLine(file);
            }

            var path = NamesConverter.GetShortPath(Utils.Path(RootDirectory, 
                                                              Utils.FileName(name, 
                                                                             ControlFileExtension)));

            System.IO.File.WriteAllText(path, 
                                        buffer.ToString());
            return "@" + path;
        }

        protected bool OnCreateExecutableMessage(CreateExecutableMessage message)
        {
            var outputBuffer = new StringBuilder();
            var process = new Process("cpe2x.exe", "/CE", NamesConverter.GetShortName(Utils.FileName(Utils.GetFileName(OutputFileName), 
                                                                                                     BinaryFileExtension)));
            var returnCode = process.Run(Logger, true, NamesConverter.GetShortPath(OutputDirectory));
            if (returnCode != 0)
            {
                outputBuffer.Append("CPE2X : error : ");
            }
            outputBuffer.AppendLine(process.Output);

            var resultMessage = new CreatingExecutableResultMessage();
            resultMessage.ReturnCode = returnCode;
            resultMessage.Output = outputBuffer.ToString();

            Server.SendMessage(resultMessage);
            return true;
        }

        protected bool OnDownloadProjectBinariesMessage(DownloadProjectBinariesMessage message)
        {
            var files = System.IO.Directory.GetFiles(NamesConverter.GetShortPath(OutputDirectory));

            foreach(var file in files)
            {
                var fileUploadMessage = new FileUploadMessage();
                fileUploadMessage.FileName = Utils.ConvertPathToLocal(NamesConverter.GetLongPath(file), OutputDirectory);
                fileUploadMessage.File     = System.IO.File.ReadAllBytes(file);
                Server.SendMessage(fileUploadMessage);
            }

            Server.SendMessage(new ProjectBinariesDownloadedMessage());
            return true;
        }
    }
}
