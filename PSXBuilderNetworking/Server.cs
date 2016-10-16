using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilderNetworking
{
    public class Server : CommunicationFramework.Server
    {
        public override void Inititalize(string address, CommunicationFramework.IDeviceLog deviceListener = null)
        {
            base.Inititalize(address, deviceListener);
            RegisterDelegates();
        }

        protected void RegisterDelegates()
        {

        }
    }
}
