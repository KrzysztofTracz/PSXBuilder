using ApplicationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilderNetworking
{
    public class Server : CommunicationFramework.Server
    {
        public override void Inititalize(string address, ILogger logger = null)
        {
            base.Inititalize(address, logger);
            RegisterDelegates();
        }

        protected void RegisterDelegates()
        {

        }
    }
}
