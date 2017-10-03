using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Collections.Concurrent;
using log4net.Repository.Hierarchy;
using System.IO;

namespace Tornado14.Utils.Net
{

    public class ClientStatusArgs
    {
        public EndPoint EndPoint { get; private set; }
        public ClientStatusArgs(EndPoint endPoint)
        {
            EndPoint = endPoint;
        }
    }

    public class ClientPackageReceivedArgs
    {
        public ConnectorBase Connector { get; private set; }
        public ConcurrentQueue<Package> Queue { get; private set; }
        public ClientPackageReceivedArgs(ConnectorBase Connector, ConcurrentQueue<Package> queue)
        {
            Connector = Connector;
            Queue = queue;
        }
    }


    public class TornadoObserver
    {
        private static readonly log4net.ILog log1 = log4net.LogManager.GetLogger("FileAppender1");
        private static readonly log4net.ILog log2 = log4net.LogManager.GetLogger("FileAppender2");
        private static readonly log4net.ILog log3 = log4net.LogManager.GetLogger("FileAppender3");
        private static readonly log4net.ILog log4 = log4net.LogManager.GetLogger("FileAppender4");
        private static readonly log4net.ILog log5 = log4net.LogManager.GetLogger("FileAppender5");
        private static readonly log4net.ILog log6 = log4net.LogManager.GetLogger("FileAppender6");

        private TcpListener tcpListener;
        private Thread listenThread;
        Dictionary<ConnectorBase, ConcurrentQueue<Package>> connectedConnectors = new Dictionary<ConnectorBase, ConcurrentQueue<Package>>();
        private static List<CancellationTokenSource> cancelationTokenList = new List<CancellationTokenSource>();
        public void Start(string file)
        {
            string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "Log4net.config";
            FileInfo finfo = new FileInfo(logFilePath);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(finfo); 

            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();

            

            CancellationTokenSource ct1 = new CancellationTokenSource();
            Task.Factory.StartNew(() => ReadClientConnectors(ct1.Token)).ContinueWith(
                antecedant =>
                {
                    foreach (KeyValuePair<ConnectorBase, ConcurrentQueue<Package>> queue in connectedConnectors)
                    {
                        queue.Key.Close();
                    }
                }
            );
            cancelationTokenList.Add(ct1);
            if (file != null && file.Length > 0)
            {
                CancellationTokenSource ct2 = new CancellationTokenSource();
                Task.Factory.StartNew(() => ReadFileRecord(file,ct2.Token)).ContinueWith(
                    antecedant =>
                    {
                    }
                );
                cancelationTokenList.Add(ct2);
            }
        }

        #region events
        public delegate void LogEventHandler(object sender, LogEventArgs e);
        public event LogEventHandler LogEvent;

        public delegate void ClientConnectedEventHandler(object sender, ClientStatusArgs e);
        public event ClientConnectedEventHandler ClientConnectedEvent;

        public delegate void ClientDisconnectedEventHandler(object sender, ClientStatusArgs e);
        public event ClientDisconnectedEventHandler ClientDisconnectedEvent;

        public delegate void ClientPackageReceivedEventHandler(object sender, ClientPackageReceivedArgs e);
        public event ClientPackageReceivedEventHandler ClientPackageReceivedEvent;

        protected virtual void RaiseLogEvent(LogType type, string text)
        {
            if (LogEvent != null)
                LogEvent(this, new LogEventArgs(type,text));
        }
        protected virtual void RaiseClientConnectedEvent(EndPoint endPoint)
        {
            if (ClientConnectedEvent != null)
                ClientConnectedEvent(this, new ClientStatusArgs(endPoint));
        }
        protected virtual void RaiseClientDisconnectedEvent(EndPoint endPoint)
        {
            if (ClientDisconnectedEvent != null)
                ClientDisconnectedEvent(this, new ClientStatusArgs(endPoint));
        }

        protected virtual void RaiseClientPackageReceivedEvent(ConnectorBase Connector, ConcurrentQueue<Package> queue)
        {
            if (ClientPackageReceivedEvent != null)
                ClientPackageReceivedEvent(this, new ClientPackageReceivedArgs(Connector, queue));
        }


        #endregion
        
        private void ListenForClients()
        {
            RaiseLogEvent(LogType.Info, "Listener Started.");

            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.tcpListener.Start();
            while (true)
            {
                if (!tcpListener.Pending())
                {
                    Thread.Sleep(500); // choose a number (in milliseconds) that makes sense
                    continue; // skip to next iteration of loop
                }
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();
                ConnectorBase Connector = new ConnectorTcp(client);
                ConcurrentQueue<Package> clientQueue = new ConcurrentQueue<Package>();
                lock (connectedConnectors)
                {
                    connectedConnectors.Add(Connector, clientQueue);
                }
                RaiseLogEvent(LogType.Info, string.Format("Client connected: {0}", client.Client.RemoteEndPoint));
                RaiseClientConnectedEvent(client.Client.RemoteEndPoint);


                //this.ClientPackageReceivedEvent += TornadoObserver_ClientPackageReceivedEvent;
                //void TornadoObserver_ClientPackageReceivedEvent(object sender, ClientPackageReceivedArgs e)
                //{
                //    SendMessageToClient(e.t14Lab.MessageTree.Connector, e.Queue);
                //}

                CancellationTokenSource ct3 = new CancellationTokenSource();
                Task.Factory.StartNew(() => SendMessageToClient(Connector, clientQueue, ct3.Token)).ContinueWith(
                    antecedant =>
                    {
                        RaiseLogEvent(LogType.Info, "SendMessageToClient Thread Stopped for Client");
                    }
                );
                cancelationTokenList.Add(ct3);
            }
        }


        private void ReadFileRecord(string file, CancellationToken ct)
        {
            Package package = new Package();
            ConnectorFile fileConnector = new ConnectorFile(file);
            fileConnector.Connect();
            while (true)
            {
                bool addMessageInPool = true;
                lock (connectedConnectors)
                {
                    foreach (KeyValuePair<ConnectorBase, ConcurrentQueue<Package>> queue in connectedConnectors)
                    {
                        if (queue.Value.Count > 10000)
                        {
                            addMessageInPool = false;
                        }
                    }

                    if (fileConnector.ReceiveCircular(out package) && addMessageInPool)
                    {
                        foreach (KeyValuePair<ConnectorBase, ConcurrentQueue<Package>> queue in connectedConnectors)
                        {
                            lock (queue.Value)
                            {
                                queue.Value.Enqueue(package);
                            }
                        }
                    }
                }
                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private void ReadClientConnectors(CancellationToken ct)
        {
            RaiseLogEvent(LogType.Info, "Read Client t14Lab.MessageTree.Connectors Thread started.");
            Package package = new Package();
            bool first = true;
            var sw = new Stopwatch();
            int packagesCount = 0;
            while (true)
            {
                try
                {
                    //bool addMessageInPool = true;
                    //lock (connectedt14Lab.MessageTree.Connectors)
                    //{
                    //    foreach (KeyValuePair<t14Lab.MessageTree.ConnectorBase, ConcurrentQueue<Package>> queue in connectedt14Lab.MessageTree.Connectors)
                    //    {
                    //        if (queue.Value.Count > 10000)
                    //        {
                    //            addMessageInPool = false;
                    //            //RaiseLogEvent(LogType.Warning, "Client buffer has 10000 packages");
                    //        }
                    //    }
                    //}
                    
                    //if (addMessageInPool)
                    //{
                        lock (connectedConnectors)
                        {
                            int sleep = 0;
                            foreach (KeyValuePair<ConnectorBase, ConcurrentQueue<Package>> readConnector in connectedConnectors)
                            {
                                if (readConnector.Key.Receive(out package))
                                {
                                    sleep--;
                                    packagesCount++;
                                    //if (first)
                                    //{
                                    //    sw.Start();
                                    //    first = false;
                                    //}
                                    // TODO [log4net] LOG 2 ReadClientt14Lab.MessageTree.Connectors(): message received
                                    //log2.Debug(package);

                                    //if (packagesCount == 700)
                                    //{
                                    //    sw.Stop();
                                    //    log2.Debug(string.Format("{0} Packages in {1} Milliseconds ~ {2} packages per millisecond and {3} packages per second.", packagesCount, sw.ElapsedMilliseconds, packagesCount / sw.ElapsedMilliseconds, (packagesCount / sw.ElapsedMilliseconds) * 1000));
                                    //}

                                    // TODO Send to extern
                                    
                                    Parallel.ForEach(connectedConnectors, (sendConnector) =>
                                    {
                                        if (sendConnector.Key != readConnector.Key)
                                        {
                                            sendConnector.Value.Enqueue(package);
                                            //RaiseClientPackageReceivedEvent(sendt14Lab.MessageTree.Connector.Key, sendt14Lab.MessageTree.Connector.Value);
                                            // TODO [log4net] LOG 3 ReadClientt14Lab.MessageTree.Connectors(): enqueue message
                                            //log3.Debug(package);
                                        }
                                    });
                                     
                                }
                                else
                                {
                                    sleep++;
                                    
                                }
                            }
                            if (sleep == connectedConnectors.Count)
                            {
                                Thread.Sleep(1);
                            }
                        }
                    //}

                }
                catch (Exception ex)
                {
                    RaiseLogEvent(LogType.Error, string.Format("TornadoObserver.ReadClientt14Lab.MessageTree.Connectors Thread Exception: {0}", ex.Message));
                    RaiseLogEvent(LogType.Info, "Read Client t14Lab.MessageTree.Connectors Thread stopped.");
                }
                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private void SendMessageToClient(ConnectorBase sendConnector, ConcurrentQueue<Package> queue, CancellationToken ct)
        {

            RaiseLogEvent(LogType.Info, "SendMessageToClient Thread started.");
            while (true)
            {
                
                Package package = null;
                bool result = queue.TryDequeue(out package);
                if (result)
                {
                    try
                    {
                        // TODO [log4net] LOG 4 SendMessageToClient() 
                        //log4.Debug(package.ToString());
                        sendConnector.Send(package);
                    }
                    catch (Exception ex)
                    {
                        lock (connectedConnectors)
                        {
                            connectedConnectors.Remove(sendConnector);
                        }
                        string clientId = "Unknown";
                        if (sendConnector.GetType() == typeof(ConnectorTcp))
                        {
                            clientId = string.Format("{0}", ((ConnectorTcp)sendConnector).Client.Client.RemoteEndPoint);
                            RaiseClientDisconnectedEvent(((ConnectorTcp)sendConnector).Client.Client.RemoteEndPoint);
                        }
                        RaiseLogEvent(LogType.Warning, string.Format("We lost '{0}' him.\r\nException:{1}", clientId, ex));

                        RaiseLogEvent(LogType.Info, "SendMessageToClient Thread stopped.");
                        return;
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }
        }


        public void Stop()
        {
            foreach (CancellationTokenSource ct in cancelationTokenList)
            {
                ct.Cancel();
            }
            listenThread.Abort();
        }
    }
}
