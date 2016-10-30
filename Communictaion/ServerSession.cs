using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationFramework;

namespace CommunicationFramework
{
    public abstract class ServerSession<T> where T : Server
    {
        public T       Server { get; protected set; }
        public ILogger Logger { get; protected set; }

        public ServerSession()
        {
            Server = null;
            Logger = null;
        }

        public    abstract void Start(); 
        protected abstract void UnsafeRegisterDelegates();
        protected abstract void UnsafeUnregisterDelegates();

        public virtual void Initialize(T       server,
                                       ILogger logger)
        {
            Server = server;
            Logger = logger;

            RegisterDelegates();
        }

        protected virtual void OnConnectionClosed()
        {
            UnregisterDelegates();
        }

        protected void RegisterDelegates()
        {
            if(!_registered)
            {
                UnsafeRegisterDelegates();
                Server.OnConnectionClosedEvent += OnConnectionClosed;
                _registered = true;
            }
        }

        protected void UnregisterDelegates()
        {
            if (_registered)
            {
                UnsafeUnregisterDelegates();
                Server.OnConnectionClosedEvent -= OnConnectionClosed;
                _registered = false;
            }
        }

        private bool _registered = false;
    }
}
