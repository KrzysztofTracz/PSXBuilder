using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CommunicationFramework
{
    public class Client : Device
    {
        public bool Connect()
        {
            return OpenConnection(new TcpClient(IPAdress.ToString(), Port));
        }
    }
}
