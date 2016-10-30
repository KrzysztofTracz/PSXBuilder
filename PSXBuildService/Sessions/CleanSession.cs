using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSXBuilderNetworking.Messages;

namespace PSXBuildService
{
    public class CleanSession : Session
    {
        public override void Start()
        {
            Logger.Log("Clean session started. User {0}, Project {1}", User, Project);
            var directory = NamesConverter.GetShortPath(RootDirectory);
            if (System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.Delete(directory, true);
            }            
            Server.SendMessage(new CleanSessionFinishedMessage());
        }

        protected override void UnsafeRegisterDelegates()
        {
            return;
        }

        protected override void UnsafeUnregisterDelegates()
        {
            return;
        }
    }
}
