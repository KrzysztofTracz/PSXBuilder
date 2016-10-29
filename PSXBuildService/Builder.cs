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
    public class Builder : ServerModule<Server>
    {
        public String User    { get; protected set; }
        public String Project { get; protected set; }

        public String RootDirectory { get; protected set; }

        public Builder()
        {
            User    = "";
            Project = "";
        }

        public void Initialize(String user,
                               String project,
                               String rootDirectory,
                               Server server,
                               ILogger logger)
        {
            base.Initialize(server, logger);

            User          = user;
            Project       = project;
            RootDirectory = Utils.Path(rootDirectory, User, Project);

            Server.SendMessage(new BuildSessionStartedMessage());

            Logger.Log("Build session started. User {0}, Project {1}", User, Project);
        }

        protected override void UnsafeRegisterDelegates()
        {
            Server.RegisterDelegate<RemoveFilesMessage>(OnRemoveFilesMessage);
            Server.RegisterDelegate<ProjectFileUploadMessage>(OnProjectFileUploadMessage);
            Server.RegisterDelegate<CompilationStartMessage>(OnCompilationStartMessage);
        }

        protected override void UnsafeUnregisterDelegates()
        {
            Server.RegisterDelegate<RemoveFilesMessage>(OnRemoveFilesMessage);
            Server.RegisterDelegate<ProjectFileUploadMessage>(OnProjectFileUploadMessage);
            Server.RegisterDelegate<CompilationStartMessage>(OnCompilationStartMessage);
        }

        protected bool OnRemoveFilesMessage(RemoveFilesMessage message)
        {
            return true;
        }

        protected bool OnProjectFileUploadMessage(ProjectFileUploadMessage message)
        {
            return true;
        }

        protected bool OnCompilationStartMessage(CompilationStartMessage message)
        {
            return true;
        }
    }
}
