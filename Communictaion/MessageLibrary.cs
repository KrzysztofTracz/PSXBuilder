using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommunicationFramework
{
    public class MessageLibrary
    {
        public Dictionary<Type,Message> Messages { get; protected set; }

        public bool IsInitialized { get; protected set; }

        public MessageLibrary()
        {
            IsInitialized = false;
            RegisterMessages();
            IsInitialized = true;
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

        protected void RegisterMessages()
        {
            Messages = new Dictionary<Type, Message>();

            var assembly = Assembly.GetEntryAssembly();
            var types    = assembly.GetTypes();

            Byte id = 0;
            foreach (var type in types)
            {
                if (type.IsClass && type.IsSubclassOf(typeof(Message)))
                {
                    var constructor = type.GetConstructor(new Type[] { });
                    var message     = constructor.Invoke(new object[] { }) as Message;
                    message.InitializeID(id++);
                    Messages.Add(type, message);
                }
            }
        }
    }
}
