/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemObjects.InternetPack
{
    public class ConnectionPool : IDisposable
    {
        public ConnectionPool(Binding bindingV4, Binding bindingV6)
            : base()
        {
            this.fBindingV4 = bindingV4;
            this.fBindingV6 = bindingV6;
            this.fCleanupTimer = new Timer(new TimerCallback(Cleanup), null, 30000, 30000);
            this.fMaxQueuePerHost = 5;
            this.fTimeout = 260;
        }

        public ConnectionPool()
            : this(new Binding(), new Binding(AddressFamily.InterNetworkV6))
        {
        }

        private Timer fCleanupTimer;
        private Hashtable fCache = new Hashtable();
        private Binding fBindingV4;
        private Binding fBindingV6;

        [Obsolete("Please use BindingV4 and BindingV6 instead", false)]
        public Binding Binding
        {
            get
            {
                return this.fBindingV4;
            }
        }

        public Binding BindingV4
        {
            get
            {
                return this.fBindingV4;
            }
        }

        public Binding BindingV6
        {
            get
            {
                return this.fBindingV6;
            }
        }

        public Int32 MaxQueuePerHost
        {
            get
            {
                return this.fMaxQueuePerHost;
            }
            set
            {
                this.fMaxQueuePerHost = value;
            }
        }
        private Int32 fMaxQueuePerHost;

        public Int32 Timeout
        {
            get
            {
                return this.fTimeout;
            }
            set
            {
                this.fTimeout = value;
            }
        }
        private Int32 fTimeout;

        public IConnectionFactory ConnectionFactory
        {
            get
            {
                return this.fConnectionFactory;
            }
            set
            {
                this.fConnectionFactory = value;
            }
        }
        private IConnectionFactory fConnectionFactory;

        public Type ConnectionClass
        {
            get
            {
                return this.fConnectionClass;
            }
            set
            {

                if (value != null && !value.IsSubclassOf(typeof(Connection)))
                    throw new Exception(String.Format("The assigned Type '{0}' is not a descendant of Connection", value.FullName));
                this.fConnectionClass = value;
            }
        }
        private Type fConnectionClass;

        public void Cleanup(Object state)
        {
            DateTime ExpireTime = DateTime.Now.AddSeconds(-this.fTimeout);
            lock (this.fCache)
            {
                foreach (DictionaryEntry entry in this.fCache)
                {
                    ConnectionQueue lQueue = (ConnectionQueue)entry.Value;
                    Boolean lModified = false;

                    for (Int32 i = lQueue.UnderlyingArray.Length - 1; i >= 0; i--)
                    {
                        if (lQueue.UnderlyingArray[i] != null && lQueue.UnderlyingArray[i].LastUsed < ExpireTime)
                        {
                            lModified = true;
                            lQueue.UnderlyingArray[i].Dispose();
                            lQueue.UnderlyingArray[i] = null;
                        }
                    }
                    if (lModified)
                        lQueue.RemoveNulls();
                }
            }
        }

        public virtual Connection GetConnection(EndPoint endPoint)
        {
            String lHost = endPoint.ToString();

            lock (this.fCache)
            {
                ConnectionQueue lQueue = this.fCache.ContainsKey(lHost) ? (ConnectionQueue)this.fCache[lHost] : null;
                if (lQueue != null && lQueue.Count > 0)
                    return lQueue.Dequeue();
            }

            return this.GetNewConnection(endPoint);
        }

        public virtual Connection GetNewConnection(EndPoint endPoint)
        {
            Binding lBinding;
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    lBinding = fBindingV4;
                    break;

                case AddressFamily.InterNetworkV6:
                    lBinding = fBindingV6;
                    break;

                default:
                    lBinding = new Binding(endPoint.AddressFamily);
                    break;
            }

            Connection lConnection;
            if (this.fConnectionFactory != null)
                lConnection = this.fConnectionFactory.CreateClientConnection(lBinding);
            else if (fConnectionClass != null)
                lConnection = (Connection)Activator.CreateInstance(this.fConnectionClass);
            else
                lConnection = new Connection(lBinding);

            lConnection.Connect(endPoint);
            lConnection.LastUsed = DateTime.Now;
            lConnection.Pool = this;

            return lConnection;
        }

        public virtual void ReleaseConnection(Connection connection)
        {
            String lHost = connection.OriginalEndpoint.ToString();

            if (!connection.Connected)
            {
                connection.Dispose();
                return;
            }

            lock (this.fCache)
            {
                ConnectionQueue lQueue;
                if (this.fCache.ContainsKey(lHost))
                    lQueue = (ConnectionQueue)this.fCache[lHost];
                else
                {
                    lQueue = new ConnectionQueue(this.fMaxQueuePerHost == 0 ? 8 : fMaxQueuePerHost);
                    this.fCache.Add(lHost, lQueue);
                }

                if (lQueue.Count < this.fMaxQueuePerHost || this.fMaxQueuePerHost < 1)
                {
                    ((Connection)connection).LastUsed = DateTime.Now;
                    lQueue.Enqueue((Connection)connection);
                }
                else
                {
                    connection.Dispose();
                }
            }
        }

        #region IDisposable Members
        public virtual void Dispose()
        {
            lock (this.fCache)
            {
                foreach (DictionaryEntry entry in this.fCache)
                {
                    while (((ConnectionQueue)entry.Value).Count > 0)
                    {
                        try
                        {
                            ((ConnectionQueue)entry.Value).Dequeue().Dispose();
                        }
                        catch (System.Net.Sockets.SocketException) // ignore socket errors
                        {
                        }
                    }
                    this.fCache.Clear();
                }
            }

            if (this.fCleanupTimer != null)
            {
                this.fCleanupTimer.Dispose();
                this.fCleanupTimer = null;
            }
        }
        #endregion
    }

    static class DefaultPool
    {
        static public ConnectionPool ConnectionPool
        {
            get
            {
                return fConnectionPool;
            }
        }
        static private ConnectionPool fConnectionPool = new ConnectionPool();
    }

    class ConnectionQueue
    {
        public ConnectionQueue(Int32 capacity)
        {
            this.fCapacity = capacity;
            this.fArray = new Connection[this.fCapacity];
            this.fCount = 0;
            this.fHead = 0;
            this.fTail = 0;
        }

        private Int32 fCapacity;
        private Int32 fHead;
        private Int32 fTail;

        public Connection[] UnderlyingArray
        {
            get
            {
                return this.fArray;
            }
        }
        private Connection[] fArray;

        public Int32 Count
        {
            get
            {
                return this.fCount;
            }
        }
        private Int32 fCount;

        public void RemoveNulls()
        {
            this.fHead = 0;
            this.fTail = 0;
            this.fCount = 0;

            for (Int32 i = 0; i < this.fCapacity; i++)
                if (this.fArray[i] != null)
                    this.Enqueue(this.fArray[i]);
        }

        public void Enqueue(Connection connection)
        {
            if (this.fCount == fCapacity || connection == null)
                throw new ArgumentOutOfRangeException();

            this.fArray[this.fTail] = connection;
            this.fTail = (this.fTail + 1) % this.fCapacity;
            this.fCount++;
        }

        public Connection Dequeue()
        {
            if (this.fCount == 0)
                throw new ArgumentOutOfRangeException();

            Connection lConnection = this.fArray[this.fHead];
            this.fArray[this.fHead] = null;
            this.fCount--;
            this.fHead = (this.fHead + 1) % this.fCapacity;

            return lConnection;
        }
    }
}