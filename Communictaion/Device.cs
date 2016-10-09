using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommunicationFramework
{
    public abstract class Device
    {
        public IPAddress IPAdress { get; protected set; }
        public Int32     Port     { get; protected set; }

        public bool IsConnected { get; protected set; }

        public delegate void OnMessageDelegate<T>(T message) where T : Message;

        public Device()
        {
            IPAdress    = null;
            Port        = -1;
            IsConnected = false;
        }

        public void Inititalize(String address)
        {
            InitAddress(address);
        }

        public bool SendMessage(Message message)
        {
            bool result = false;
            if(IsConnected)
            {
                _stream.Write(message.ToByteArray(), 0, message.Size);
                result = true;
            }
            return result;
        }

        public bool WaitForMessage()
        {
            bool result = false;
            if (IsConnected)
            {
                Byte[] headerBuffer = new Byte[Message.GetHeaderSize()];
                _stream.Read(headerBuffer, 0, headerBuffer.Length);

                Byte  messageID   = headerBuffer[0];
                Int16 messageSize = BitConverter.ToInt16(headerBuffer, 1);

                var message = Message.Library.GetMessageByID(messageID);

                Byte[] messageDataBuffer = new byte[messageSize];
                _stream.Read(messageDataBuffer, 0, messageDataBuffer.Length);

                message.FromByteArray(messageDataBuffer);

                if(messageDelegates.ContainsKey(message.GetType()))
                {
                    var messageDelegate = messageDelegates[message.GetType()];
                    messageDelegate.DynamicInvoke(message);
                }                

                result = true;
            }
            return result;
        }

        protected bool OpenConnection(TcpClient client)
        {
            _tcpClient  = client;
            _stream     = _tcpClient.GetStream();
            IsConnected = true;

            return true;
        }

        protected bool CloseConnection()
        {
            _stream.Close();
            _stream = null;

            _tcpClient.Close();
            _tcpClient = null;

            IsConnected = false;

            return true;
        }

        private void InitAddress(String address)
        {
            var split = address.Split(':');

            IPAdress = IPAddress.Parse(split[0]);
            Port     = Int32.Parse(split[1]);
        }

        public void RegisterDelegate<T>(OnMessageDelegate<T> onMessage) where T : Message
        {
            if(!messageDelegates.ContainsKey(typeof(T)))
            {
                messageDelegates.Add(typeof(T), onMessage);
            }
        }

        private TcpClient     _tcpClient = null;
        private NetworkStream _stream    = null;

        private Dictionary<Type, Delegate> messageDelegates = new Dictionary<Type, Delegate>();
    }
}
