using Blue.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tornado14.ProjectExplorer.BusinessObjects;
using Tornado14.Utils;
using Tornado14.Utils.Net;

namespace Tornado14.ProjectExplorer.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ExtendedObservableCollection<ClientApp> ClientAppList
        {
            get
            {
                if (dataAccess == null)
                {
                    dataAccess = new DAL();
                }
                return dataAccess.ClientAppList;
            }
        }

        private DAL dataAccess;
        private object _listLock = new object();
        TornadoObserver server = new TornadoObserver();
        private StickyWindow _stickyWindow;

        void window1Loaded(object sender, RoutedEventArgs e)
        {
            StickyWindow.RegisterExternalReferenceForm(this);
            _stickyWindow = new StickyWindow(this);
            _stickyWindow.StickToScreen = true;
            _stickyWindow.StickToOther = true;
            _stickyWindow.StickOnResize = true;
            _stickyWindow.StickOnMove = true;
        }

        public MainWindow()
        {
            InitializeComponent();
            this.MaxWidth = 10000;
            this.MaxHeight = 10000;
            this.Loaded += window1Loaded;
            dataAccess = new DAL();
            BindingOperations.EnableCollectionSynchronization(ClientAppList, _listLock);

            DataGridClients.ItemsSource = ClientAppList;

            
            server.ClientConnectedEvent += server_ClientConnectedEvent;
            server.ClientDisconnectedEvent += server_ClientDisconnectedEvent;
            server.LogEvent += server_LogEvent;
            server.Start(null);
            ClientAppList.CollectionChanged += ClientAppList_CollectionChanged;
            ClientAppList.ElementContentChanged += ClientAppList_ElementContentChanged;

            string[] args = Environment.GetCommandLineArgs();
            // Set Console Position & dimensions
            ServerArgs options = new ServerArgs();
            options = options.parseArgs(args);

            if (options.Left > 0 || options.Top > 0)
            {
                this.Left = options.Left;
                this.Top = options.Top;
            }
            if (options.Width > 0 || options.Height > 0)
            {
                this.Width = options.Width;
                this.Height = options.Height;
            }
        }

        void server_LogEvent(object sender, LogEventArgs e)
        {
            PrintLog(e);
            /*
            PrintLog(new LogEventArgs(LogType.Debug, "Debug test"));
            PrintLog(new LogEventArgs(LogType.Info, "Info test"));
            PrintLog(new LogEventArgs(LogType.Error, "Error test"));
            PrintLog(new LogEventArgs(LogType.Warning, "Warning test"));

            PrintLog(new LogEventArgs(LogType.Debug, "Debug test"));
            PrintLog(new LogEventArgs(LogType.Info, "Info test"));
            PrintLog(new LogEventArgs(LogType.Error, "Error test"));
            PrintLog(new LogEventArgs(LogType.Warning, "Warning test"));
             */
        }

        void ClientAppList_ElementContentChanged(object sender, EventArgs e)
        {
            int a = 0;
        }

        void ClientAppList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int a = 0;
        }

        delegate void PrintLogInvoker(LogEventArgs arg);

        private void PrintLog(LogEventArgs e)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new PrintLogInvoker(PrintLog), e);
                return;
            }

            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(RichTextBoxLog.Document.ContentEnd, RichTextBoxLog.Document.ContentEnd);
            tr.Text = string.Format("{0} - {1}\r",DateTime.Now.ToString("HH:mm:ss.fff"),e.Text);
            object value = null;
            
            switch (e.Type)
            {
                case LogType.Info:
                    value = bc.ConvertFromString(Colors.White.ToString());
                    break;
                case LogType.Error:
                    value = bc.ConvertFromString(Colors.Red.ToString());
                    break;
                case LogType.Warning:
                    value = bc.ConvertFromString(Colors.Orange.ToString());
                    break;
                case LogType.Debug:
                    value = bc.ConvertFromString(Colors.LightGreen.ToString());
                    break;
                default:
                    break;
            }
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, value);
            }
            catch (FormatException) { }
            RichTextBoxLog.ScrollToEnd();
        }


        void server_ClientDisconnectedEvent(object sender, ClientStatusArgs e)
        {
            lock (_listLock)
            {
                ClientApp removeApp = null;
                foreach (ClientApp app in ClientAppList)
                {
                    if (app.IPAddress == e.EndPoint.ToString())
                        removeApp = app;
                }
                ClientAppList.Remove(removeApp);
            }
        }

        void server_ClientConnectedEvent(object sender, ClientStatusArgs e)
        {
            lock (_listLock)
            {
                ClientAppList.Add(new ClientApp() { IPAddress = e.EndPoint.ToString(), Name = "name", Status = "Connected" });
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server.Stop();
            server = null;
        }
    }
}
