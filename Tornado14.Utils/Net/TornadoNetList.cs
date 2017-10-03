using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tornado14.Utils.Net
{
    public class TornadoNetList
    {
        protected int senderId;

        private static FileStream logFileFileStream = null;
        private static StreamWriter logFileStreamWriter = null;

        private static CancellationTokenSource cancelationToken;
        private static List<CancellationTokenSource> cancelationTokenList = new List<CancellationTokenSource>();

        #region events

        public delegate void LogEventHandler(object sender, LogEventArgs e);
        public event LogEventHandler LogEvent;
        protected virtual void RaiseLogEvent(LogType type, string text)
        {
            if (LogEvent != null)
                LogEvent(this, new LogEventArgs(type, text));
        }

        public delegate void CurrentItemChangedEventHandler(object sender, CurrentItemChangedEventArgs e);
        public event CurrentItemChangedEventHandler CurrentItemChangedEvent;
        protected virtual void RaiseCurrentItemChangedEvent(Guid pk)
        {
            if (CurrentItemChangedEvent != null)
                CurrentItemChangedEvent(this, new CurrentItemChangedEventArgs(pk));
        }


        #endregion

        public void Start<T>(string connectionString, int senderId, string logFile, FileInfo xmlFile)
        {
            LoadData(xmlFile);

            this.senderId = senderId;
            Filter filter = new Filter();
            filter.RecipientList = new int[] { senderId };
            
            cancelationToken = new CancellationTokenSource();
            Thread.Sleep(1001);
            ConnectorBase connector = ConnectorFactory.CreateConnector(connectionString);
            

            Task.Factory.StartNew(() => Process(connector, filter, logFile, cancelationToken.Token))
                  .ContinueWith(antecedant =>
                  {
                      RaiseLogEvent(LogType.Info, string.Format("Process Stopped"));
                      if (logFile != null && logFile != string.Empty)
                      {
                          logFileStreamWriter.Close();
                          logFileFileStream.Close();
                      }
                  });
        }


        public void Process(ConnectorBase connector, Filter filter, string logFile, CancellationToken ct)
        {
            try
            {
                
                connector.Connect();
                RaiseLogEvent(LogType.Info, string.Format("Connected to Server"));
                Package package;

                if (logFile != null && logFile != string.Empty)
                {
                    RaiseLogEvent(LogType.Info, string.Format("Log file: {0}", logFile));
                    logFileFileStream = new FileStream(logFile, FileMode.OpenOrCreate);
                    logFileStreamWriter = new StreamWriter(logFileFileStream);
                }
                bool first = true;
                int packagesCount = 0;
                var sw = new Stopwatch();

                while (true)
                {
                    Thread.Sleep(1);
                    while (connector.Receive(out package))
                    {
                        packagesCount++;
                        if (first)
                        {
                            sw.Start();
                            first = false;
                        }
                        try
                        {
                            bool processMessage = filter.applyFilter(package);
                            if (processMessage)
                            {
                                package.PrintToConsole(packagesCount, ConsoleColor.Green);

                                LogType infoLogType = LogType.Info;
                                switch (package.Method)
                                {
                                    case Method.Response:
                                        infoLogType = LogType.InfoRed;
                                        break;
                                    case Method.GET:
                                        infoLogType = LogType.InfoBlue;
                                        break;
                                    case Method.POST:
                                        infoLogType = LogType.InfoGreen;
                                        break;
                                    case Method.PUT:
                                        infoLogType = LogType.InfoYellow;
                                        break;
                                    case Method.DELETE:
                                        infoLogType = LogType.InfoMagenta;
                                        break;
                                    default:
                                        break;
                                }

                                RaiseLogEvent(infoLogType, string.Format("{0} {1}", packagesCount.ToString().PadRight(7), package.ToStringShort()));

                                switch (package.Method)
                                {
                                    case Method.GET:
                                        ProcessGET(connector, package);
                                        Package eofPackage = new Package(senderId, package.Sender, 2, Method.Response, null);
                                        connector.Send(eofPackage);
                                        // TODO updates while sending hole list ?
                                        break;
                                    case Method.POST:
                                        ProcessPOST(connector, package);
                                        Package postOKPackage = new Package(senderId, package.Sender, 1, Method.Response, null);
                                        //connector.Send(postOKPackage);

                                        break;
                                    case Method.PUT:
                                        ProcessPUT(connector, package);
                                        Package putOKPackage = new Package(senderId, package.Sender, 1, Method.Response, null);
                                        //connector.Send(putOKPackage);
                                        break;
                                    case Method.DELETE:
                                        ProcessDELETE(connector, package);
                                        Package deleteOKPackage = new Package(senderId, package.Sender, 1, Method.Response, null);
                                        //connector.Send(deleteOKPackage);
                                        break;
                                    default:
                                        break;
                                }


                                if (logFileStreamWriter != null)
                                {
                                    logFileStreamWriter.WriteLine("{0}", package.ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            RaiseLogEvent(LogType.Error, "TornadoNetList.Process() Process Message: " + ex.Message);
                            return;
                        }

                        if (ct.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                RaiseLogEvent(LogType.Error, "TornadoNetList.Process(): " + e.Message);
            }

            return;
        }

        protected virtual void LoadData(FileInfo xmlFile)
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.LoadData() Not Implemented");
        }
        public virtual void AutoSave()
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.AutoSave() Not Implemented");
        }

        public virtual void SaveData(FileInfo xmlFile)
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.SaveData() Not Implemented");
        }

        protected virtual void ProcessGET(ConnectorBase connector, Package package)
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.ProcessGET() Not Implemented");
        }

        protected virtual void ProcessPUT(ConnectorBase connector, Package package)
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.ProcessPUT() Not Implemented");
        }

        protected virtual void ProcessPOST(ConnectorBase connector, Package package)
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.ProcessPOST() Not Implemented");
        }
        protected virtual void ProcessDELETE(ConnectorBase connector, Package package)
        {
            RaiseLogEvent(LogType.Error, "TornadoNetList.ProcessDELETE() Not Implemented");
        }

        public void Stop()
        {
            foreach (CancellationTokenSource ct in cancelationTokenList)
            {
                ct.Cancel();
            }
            cancelationToken.Cancel();
            RaiseLogEvent(LogType.Info, "Program Stoped.");
        }
    }
}



/*
int count = 0;
StringBuilder bundle = new StringBuilder();
bundle.Append("<ArrayOfTodo>");
foreach (Todo todo in dataTaskList)
{
    bundle.Append(XmlSerializationHelper.Serialize(todo));
    count++;
    if (count == 5) {
        bundle.Append("</ArrayOfTodo>");
        Package todoPackage = new Package(SENDERID, package.Sender, 9, Method.Response, bundle.ToString());
        connector.Send(todoPackage);
        bundle.Clear();
        bundle.Append("<ArrayOfTodo>");
        count = 0;
    }
}
if (bundle.Length > 0)
{
    bundle.Append("</ArrayOfTodo>");
    Package todoPackage = new Package(SENDERID, package.Sender, 10, Method.Response, bundle.ToString());
    connector.Send(todoPackage);
}
*/
