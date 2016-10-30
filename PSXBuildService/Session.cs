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
    public abstract class Session : ServerSession<Server>
    {
        public String User          { get; protected set; }
        public String Project       { get; protected set; }
        public String RootDirectory { get; protected set; }

        public DOSNamesConverter NamesConverter { get; protected set; }

        public Session()
        {
            User          = "";
            Project       = "";
            RootDirectory = "";

            NamesConverter = null;
        }

        public virtual void Initialize(String user,
                                       String project,
                                       String rootDirectory,
                                       Server server,
                                       ILogger logger)
        {
            base.Initialize(server, logger);

            User    = user;
            Project = project;

            RootDirectory = Utils.Path(rootDirectory, User, Project);

            NamesConverter = new DOSNamesConverter();
            NamesConverter.Initialize(logger);
            NamesConverter.Load();
        }

        protected String GetFullPath(String localPath)
        {
            return Utils.Path(RootDirectory, localPath);
        }
    }
}
