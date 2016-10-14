using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilderNetworking
{
    public class Client : CommunicationFramework.Client
    {
        public override void Inititalize(string address)
        {
            base.Inititalize(address);
            RegisterDelegates();
        }

        protected void RegisterDelegates()
        {

        }
    }
}
