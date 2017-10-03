using Addison_Wesley.Codebook.WPF;
using Blue.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Tornado14.Utils.Net;

namespace Tornado14.ProjectExplorer.Collector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static FileInfo taskFile;
        private static FileInfo projectFile;
        private static TodoNetList taskNetList;
        private static ProjectNetList projectNetList;

        private StickyWindow _stickyWindow;

        void window1Loaded(object sender, RoutedEventArgs e)
        {
            _stickyWindow = new StickyWindow(this);
            _stickyWindow.StickToScreen = true;
            _stickyWindow.StickToOther = true;
            _stickyWindow.StickOnResize = true;
            _stickyWindow.StickOnMove = true;
        }

        public MainWindow()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;

            this.MaxWidth = 10000;
            this.MaxHeight = 10000;
            this.Loaded += window1Loaded;
            CollectorArgs options = CollectorArgs.ParseArgs(Environment.GetCommandLineArgs());
            try
            {
                //SetWindowPositionAndDimensions(options);
                string[] files = options.DataFile.Split(',');
                string[] senderIds = options.SenderId.Split(',');

                int taskId = int.Parse(senderIds[0]);
                taskFile = new FileInfo(files[0]);
                int projectId = int.Parse(senderIds[1]);
                projectFile = new FileInfo(files[1]);
                this.Title = string.Format("[{0},{1}] - {2}", taskId, projectId , this.Title);

                taskNetList = new TodoNetList();
                taskNetList.LogEvent += taskNetList_LogEvent;
                taskNetList.CurrentItemChangedEvent += taskNetList_CurrentItemChangedEvent;
                projectNetList = new ProjectNetList();
                projectNetList.LogEvent += projectNetList_LogEvent;
                projectNetList.CurrentItemChangedEvent += projectNetList_CurrentItemChangedEvent;
                taskNetList.Start<Todo>(options.ConnectionString, taskId, options.LogFile, taskFile);
                projectNetList.Start<Project>(options.ConnectionString, projectId, options.LogFile, projectFile);

                System.Timers.Timer autoSaveTimer = new System.Timers.Timer();
                autoSaveTimer.Elapsed += autoSaveTimer_Elapsed;
                autoSaveTimer.Interval = 5000;
                autoSaveTimer.Enabled = true;

                System.Timers.Timer halfHourSaveTimer = new System.Timers.Timer();
                halfHourSaveTimer.Elapsed += halfHourSaveTimer_Elapsed;
                halfHourSaveTimer.Interval = 1800000;
                halfHourSaveTimer.Enabled = true;

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

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Start Exception: {0}", ex.Message));
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
        }

        public static readonly DependencyProperty CurrentTodoProperty = DependencyProperty.Register("CurrentTodo", typeof(Todo), typeof(MainWindow), null);

        public Todo CurrentTodo
        {
            get { return (Todo)GetValue(CurrentTodoProperty); }
            set { SetValueDp(CurrentTodoProperty, value); }
        }

        public static readonly DependencyProperty CurrentProjectProperty = DependencyProperty.Register("CurrentProject", typeof(Project), typeof(MainWindow), null);

        public Project CurrentProject
        {
            get { return (Project)GetValue(CurrentProjectProperty); }
            set { SetValueDp(CurrentProjectProperty, value); }
        }

        void projectNetList_CurrentItemChangedEvent(object sender, CurrentItemChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                CurrentProject = projectNetList.DataList[e.PK];
            }));

        }

        void taskNetList_CurrentItemChangedEvent(object sender, CurrentItemChangedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                CurrentTodo = taskNetList.DataList[e.PK];
            }));

        }

        void projectNetList_LogEvent(object sender, LogEventArgs e)
        {
            PrintLog(e);
        }

        void taskNetList_LogEvent(object sender, LogEventArgs e)
        {
            PrintLog(e);
        }

        static void halfHourSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FileInfo autosaveFileTodos = new FileInfo(string.Format("{0}\\{1}_Autosave{2}", taskFile.Directory, taskFile.Name.Replace(taskFile.Extension, string.Empty), taskFile.Extension));
            FileInfo autosaveFileProjects = new FileInfo(string.Format("{0}\\{1}_Autosave{2}", projectFile.Directory, projectFile.Name.Replace(projectFile.Extension, string.Empty), projectFile.Extension));
            //FileInfo autosaveFileProjects = new FileInfo(string.Format("{0}\\{1}_{2}{3}", projectFile.Directory, projectFile.Name.Replace(projectFile.Extension, string.Empty), DateTime.Now.ToString("Autosave_yyyy-MM-dd_HH-mm"), projectFile.Extension));
            taskNetList.SaveData(autosaveFileTodos);
            projectNetList.SaveData(autosaveFileProjects);
        }

        private static void autoSaveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FileInfo autosaveFileTodos = new FileInfo(string.Format("{0}\\{1}_Autosave{2}", taskFile.Directory, taskFile.Name.Replace(taskFile.Extension, string.Empty), taskFile.Extension));
            FileInfo autosaveFileProjects = new FileInfo(string.Format("{0}\\{1}_Autosave{2}", projectFile.Directory, projectFile.Name.Replace(projectFile.Extension, string.Empty), projectFile.Extension));
            //FileInfo autosaveFileProjects = new FileInfo(string.Format("{0}\\{1}_{2}{3}", projectFile.Directory, projectFile.Name.Replace(projectFile.Extension, string.Empty), DateTime.Now.ToString("Autosave_yyyy-MM-dd_HH-mm"), projectFile.Extension));
            taskNetList.SaveData(autosaveFileTodos);
            projectNetList.SaveData(autosaveFileProjects);
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
            tr.Text = string.Format("{0} - {1}\r", DateTime.Now.ToString("HH:mm:ss.fff"), e.Text);
            object value = null;

            switch (e.Type)
            {
                case LogType.Info:
                    value = bc.ConvertFromString(Colors.White.ToString());
                    break;
                case LogType.InfoBlue:
                    value = bc.ConvertFromString(Colors.BlueViolet.ToString());
                    break;
                case LogType.InfoGreen:
                    value = bc.ConvertFromString(Colors.Green.ToString());
                    break;
                case LogType.InfoMagenta:
                    value = bc.ConvertFromString(Colors.Magenta.ToString());
                    break;
                case LogType.InfoRed:
                    value = bc.ConvertFromString(Colors.IndianRed.ToString());
                    break;
                case LogType.InfoYellow:
                    value = bc.ConvertFromString(Colors.Yellow.ToString());
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        #region Aero

        /// <summary>
        /// Stattet das Fenster mit einem erweiterten Aero-Rahmen aus
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Den Aero-Rahmen in den Innenbereich verbreitern
            WindowUtils.ExtendWindowFrame(this);

            // Die Hook-Methode in die Nachrichtenverarbeitung einhängen
            HwndSource hwndSource = HwndSource.FromHwnd(
               new WindowInteropHelper(this).Handle);
            hwndSource.AddHook(new HwndSourceHook(this.WindowProc));
        }

        /// <summary>
        /// Fängt den Wechsel des Windows-Themas ab, um den Rahmen
        /// neu zu definieren
        /// </summary>
        private IntPtr WindowProc(IntPtr hwnd, int msg,
           IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int DWMCOMPOSITIONCHANGED = 0x031E;
            if (msg == DWMCOMPOSITIONCHANGED)
            {
                // Den Rahmen neu definieren
                WindowUtils.ExtendWindowFrame(this);
            }

            // Ein leeres Ergebnis zurückgeben
            return IntPtr.Zero;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RichTextBoxLog.Document.Blocks.Clear();
        }

        private void Rectangle_DragEnter(object sender, DragEventArgs e)
        {
            int a = 0;
        }

        private void Rectangle_DragOver(object sender, DragEventArgs e)
        {
            int a = 0;
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            bool isCorrect = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                    if (info.Extension != ".txt")
                    {
                        isCorrect = false;
                        break;
                    }
                }
            } 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
