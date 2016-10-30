using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXBuilderNetworking;
using PSXBuilderNetworking.Messages;

namespace PSXBuilder
{
    class CleanProgram : ApplicationFramework.Program<PSXBuilder>
    {
        public override bool Start(params String[] arguments)
        {
            bool result = false;

            var project = new PSXProject();
            if (project.Load(arguments[0], arguments[1], arguments[2]))
            {
                BuildInfo.Clear(project.IntermediateDir);

                var client = new Client();
                client.Inititalize(Application.NetworkingSystem.GetConnectionAddress(),
                                   Application.Console);
                client.Connect();

                var user = Environment.MachineName;
                Log("Starting clean session. User: {0}, Project: {1}.", user, project.Name);
                var cleanSessionStartMessage = new CleanSessionStartMessage();
                cleanSessionStartMessage.User    = user;
                cleanSessionStartMessage.Project = project.Name;

                client.SendMessage(cleanSessionStartMessage);
                client.WaitForMessage<CleanSessionFinishedMessage>();
                Log("Clean session finished.");
                client.Disconnect();

                result = true;
            }
            return result;
        }

        protected override String[] GetArguments()
        {
            return new[] { "projectFile", "configuration", "toolsVersion" };
        }

        protected override String GetDescription()
        {
            return "clean project";
        }

        protected override String GetSpecifier()
        {
            return "-c";
        }
    }
}
