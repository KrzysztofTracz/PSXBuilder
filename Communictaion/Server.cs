﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using CommunicationFramework.Messages;

namespace CommunicationFramework
{
    public class Server : Device
    {
        public void Start()
        {
            _server = new TcpListener(IPAdress, Port);
            _server.Start();

            while(true)
            {
                OpenConnection(_server.AcceptTcpClient());
            }
        }

        private TcpListener _server = null;
    }
}
