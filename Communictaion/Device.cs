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

        public bool IsInitialized { get; protected set; }

        public bool IsConnected
        {
            get
            {
                return _tcpClient != null && _tcpClient.Connected;
            }
        }

        public delegate bool OnMessageDelegate<T>(T message) where T : Message;

        public Device()
        {
            IPAdress      = null;
            Port          = -1;
            IsInitialized = false;
        }

        public virtual void Inititalize(String address, IDeviceLog log = null)
        {
            InitAddress(address);
            RegisterDelegate<Messages.PartialMessageStart>(OnPartialMessageStart);
            RegisterDelegate<Messages.PingMessage>(OnPingMessage);
            IsInitialized = true;
            _log = log;
        }

        public bool StayConnected()
        {
            bool result = true;
            if(IsConnected)
            {
                SendSingleMessage(new Messages.HeartbeatMessage());
                _tcpClient.Client.Poll(0, SelectMode.SelectRead);
            }

            if(!IsConnected)
            {
                CloseConnection();
                result = false;
            }
            return result;
        }

        public bool SendMessage(Message message)
        {
            bool result = false;
            if(IsInitialized)
            {
                result = message.Size > Message.SizeLimit ? SendMessageInParts(message)
                                                          : SendSingleMessage(message);
            }
            return result;
        }

        public bool WaitForMessage()
        {
            Log("Waiting for message.");
            bool result = false;
            if (IsInitialized)
            {
                var message = ReadMessageFromStream();
                {
                    result = InvokeMessageDelegate(message);
                }
            }
            return result;
        }

        public T WaitForMessage<T>() where T : Message
        {
            Log("Waiting for {0}.", typeof(T).Name);
            T result = null;
            if (IsInitialized)
            {
                Message message = ReadMessageFromStream();
                while (message != null)
                {
                    if (message.GetType().Equals(typeof(T)))
                    {
                        result = message as T;
                        break;
                    }
                    else
                    {
                        InvokeMessageDelegate(message);
                        message = ReadMessageFromStream();
                    }
                }
            }            
            return result;
        }

        public virtual bool Disconnect()
        {
            return CloseConnection();
        }

        public bool Ping()
        {
            bool result = SendMessage(new Messages.PingMessage());
            if(result)
            {
                WaitForMessage<Messages.PongMessage>();
            }
            return result;
        }

        protected virtual bool OnPingMessage(Messages.PingMessage message)
        {
            SendMessage(new Messages.PongMessage());
            return true;
        }

        protected bool InvokeMessageDelegate(Message message)
        {
            bool result = false;
            if (message != null &&
                messageDelegates.ContainsKey(message.GetType()))
            {
                var messageDelegate = messageDelegates[message.GetType()];
                result = (bool)messageDelegate.DynamicInvoke(message);
            }
            return result;
        }

        protected Message ReadMessageFromStream()
        {
            Message result = null;
            if (IsConnected)
            {
                Byte[] headerBuffer = new Byte[Message.GetHeaderSize()];
                _stream.Read(headerBuffer, 0, headerBuffer.Length);

                Byte messageID = headerBuffer[0];
                int messageSize = BitConverter.ToInt32(headerBuffer, 1) - Message.GetHeaderSize();

                result = Message.Library.GetMessageByID(messageID);

                if (messageSize > 0)
                {
                    Byte[] messageDataBuffer = new byte[messageSize];
                    _stream.Read(messageDataBuffer, 0, messageDataBuffer.Length);
                    result.FromByteArray(messageDataBuffer);
                }

                Log("{0} received.", result.GetName());
            }
            return result;
        }

        protected Message GetMessageFromBuffer(Byte[] buffer)
        {
            Message result = null;
            if(buffer.Length >= Message.GetHeaderSize())
            {
                Byte messageID  = buffer[0];
                int messageSize = BitConverter.ToInt32(buffer, 1);

                result = Message.Library.GetMessageByID(messageID);

                Byte[] messageDataBuffer = new byte[messageSize];
                for(int i = Message.GetHeaderSize(); i < buffer.Length; i++)
                {
                    messageDataBuffer[i - Message.GetHeaderSize()] = buffer[i];
                }
                result.FromByteArray(messageDataBuffer);
            }
            return result;
        }

        protected Byte[] PartialMessageDataBuffer = null;

        protected Byte[] GetPartialMessageDataFromBuffer()
        {
            var size = PartialMessageDataBuffer.Length > Message.SizeLimit ? Message.SizeLimit
                                                                           : PartialMessageDataBuffer.Length;
            var result = new Byte[size];
            for(int i=0;i<size;i++)
            {
                result[i] = PartialMessageDataBuffer[i];
            }

            var newBufferSize = PartialMessageDataBuffer.Length - size;
            var newBuffer = new Byte[newBufferSize];
            for(int i=size;i<PartialMessageDataBuffer.Length;i++)
            {
                newBuffer[i - size] = PartialMessageDataBuffer[i];
            }
            PartialMessageDataBuffer = newBuffer;

            return result;
        }
        
        protected bool SendMessageInParts(Message message)
        {
            bool result = false;
            if (IsConnected)
            {
                PartialMessageDataBuffer = message.ToByteArray();
                int parts = (PartialMessageDataBuffer.Length / Message.SizeLimit) + 1;

                var partialMessageStart = new Messages.PartialMessageStart();
                partialMessageStart.Parts     = parts;
                partialMessageStart.TotalSize = PartialMessageDataBuffer.Length;

                Log("Sending {0} in {1} parts [{2} bytes]",
                                 message.GetName(),
                                 partialMessageStart.Parts,
                                 partialMessageStart.TotalSize);

                SendSingleMessage(partialMessageStart);
                WaitForMessage<Messages.PartialMessageReceived>();

                while (PartialMessageDataBuffer.Length > 0)
                {
                    Log("{0} bytes left",
                                     PartialMessageDataBuffer.Length);

                    var partialMessage = new Messages.PartialMessage();
                    partialMessage.Data = GetPartialMessageDataFromBuffer();
                    SendSingleMessage(partialMessage);
                    WaitForMessage<Messages.PartialMessageReceived>();
                }
                result = true;
            }
            return result;
        }

        protected bool SendSingleMessage(Message message)
        {
            if (IsConnected)
            {
                Log("Sending {0}", message.GetName());
                _stream.Write(message.ToByteArray(), 0, message.Size);
            }
            return true;
        }

        protected virtual bool OnPartialMessageStart(Messages.PartialMessageStart message)
        {
            int parts  = message.Parts;
            var buffer = new Byte[message.TotalSize];
            int cursor = 0;

            Log("Receiving message in {0} parts [{1} bytes]",
                             message.Parts,
                             message.TotalSize);

            SendSingleMessage(new Messages.PartialMessageReceived());

            for(int part=0;part<parts;part++)
            {
                var partialMessage = WaitForMessage<Messages.PartialMessage>();
                for(int i=0;i<partialMessage.Data.Length;i++,cursor++)
                {
                    buffer[cursor] = partialMessage.Data[i];
                }

                Log("{0} parts received [{1} bytes]",
                                 part + 1,
                                 cursor);
                    
                SendSingleMessage(new Messages.PartialMessageReceived());
            }

            var receivedMessage = GetMessageFromBuffer(buffer);
            Log("{0} received.", receivedMessage.GetName());

            return InvokeMessageDelegate(receivedMessage);
        }

        protected bool OpenConnection(TcpClient client)
        {
            Log("Opening connection.");
            _tcpClient  = client;
            _stream     = _tcpClient.GetStream();
            return true;
        }

        protected bool CloseConnection()
        {
            Log("Closing connection.");
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }

            if (_tcpClient != null)
            {
                _tcpClient.Close();
                _tcpClient = null;
            }
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

        protected void Log(String format, params object[] objects)
        {
            Log(String.Format(format, objects));
        }

        protected void Log(String text)
        {
            if(_log != null)
            {
                _log.WriteLine(text);
            }
        }

        private TcpClient       _tcpClient = null;
        private NetworkStream   _stream    = null;
        private IDeviceLog      _log       = null;

        private Dictionary<Type, Delegate> messageDelegates = new Dictionary<Type, Delegate>();
    }
}
