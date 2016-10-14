using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommunicationFramework
{
    public class MessageLibrary
    {
        public static MessageLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageLibrary();
                    instance.RegisterMessages();
                }
                return instance;
            }
        }

        public Dictionary<Type,Message> Messages { get; protected set; }
        public bool IsInitialized { get; set; }

        public MessageLibrary()
        {
            IsInitialized = false;
            Messages      = new Dictionary<Type, Message>();
        }

        public Byte GetID(Type messageType)
        {
            return Messages[messageType].ID;
        }

        public Message GetMessageByID(Byte id)
        {
            Message result      = null;
            Type    messageType = null;

            var messages = Messages.Values;
            foreach(var message in messages)
            {
                if(message.ID == id)
                {
                    messageType = message.GetType();
                    break;
                }
            }

            if(messageType != null)
            {
                var constructor = messageType.GetConstructor(new Type[] { });
                result = constructor.Invoke(new object[] { }) as Message;
            }

            return result;
        }

        public void RegisterMessages()
        {
            var assembly = Assembly.GetCallingAssembly();
            var types    = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.IsClass && type.IsSubclassOf(typeof(Message)))
                {
                    var constructor = type.GetConstructor(new Type[] { });
                    var message     = constructor.Invoke(new object[] { }) as Message;
                    RegisterMessage(message);
                }
            }
        }

        protected void RegisterMessage(Message message)
        {
            message.InitializeID(lastID++);
            Messages.Add(message.GetType(), message);
        }

        protected static MessageLibrary instance = null;

        protected Byte lastID   = 0;
    }
}
