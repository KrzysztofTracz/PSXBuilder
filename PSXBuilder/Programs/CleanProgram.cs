using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;

namespace PSXBuilder
{
    class CleanProgram : ProjectProgram
    {
        public override bool Start()
        {
            bool result = false;

            var project = new PSXProject();
            if (project.Load(Project, Configuration, Tools))
            {
                BuildInfo.Clear(project.IntermediateDir);
                if (System.IO.Directory.Exists(project.OutputDir))
                {
                    System.IO.Directory.Delete(project.OutputDir, true);
                }

                var client = new Client();
                client.Inititalize(Application.NetworkingSystem.GetConnectionAddress(),
                                   Application.Console);
                client.Connect();

                var user = Environment.MachineName;
                Log("Starting clean session. User: {0}, Project: {1}.", user, project.Name);
                var cleanSessionStartMessage = new CleanSessionStartMessage();
                cleanSessionStartMessage.User          = user;
                cleanSessionStartMessage.Project       = project.Name;
                cleanSessionStartMessage.Configuration = project.Configuration;

                client.SendMessage(cleanSessionStartMessage);
                client.WaitForMessage<CleanSessionFinishedMessage>();
                Log("Clean session finished.");
                client.Disconnect();

                result = true;
            }
            return result;
        }

        protected override String GetDescription()
        {
            return "Clean project";
        }

        protected override String GetSpecifier()
        {
            return "c";
        }
    }
}
