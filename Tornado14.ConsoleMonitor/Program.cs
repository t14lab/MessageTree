using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tornado14.Utils.Net;

namespace Tornado14.ConsoleMonitor
{
    class Program
    {
        #region Set Window Position Magic

        const int SWP_NOSIZE = 0x0001;
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr MyConsole = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        #endregion

        private static FileStream logFileFileStream = null;
        private static StreamWriter logFileStreamWriter = null;
        private static CancellationTokenSource cancelationToken;

        static void Main(string[] args)
        {
            ConsoleMonitorArgs options = ConsoleMonitorArgs.ParseArgs(args);

            SetWindowPositionAndDimensions(options);
            Filter filter = new Filter();
            filter.EventList = Filter.ParseCommaSeparatedIntegers(options.Event);
            filter.MethodList = Filter.ParseCommaSeparatedIntegers(options.Method);
            filter.RecipientList = Filter.ParseCommaSeparatedIntegers(options.Recipient);
            filter.SenderList = Filter.ParseCommaSeparatedIntegers(options.Sender);


            Console.ForegroundColor = ConsoleColor.Green;
            cancelationToken = new CancellationTokenSource();
            Thread.Sleep(1000);
            Task.Factory.StartNew(() => Process(options.ConnectionString, filter, options.LogFile, cancelationToken.Token))
                  .ContinueWith(antecedant =>
                  {
                      Console.WriteLine("Process Stopped.");
                      if (options.LogFile != null && options.LogFile != string.Empty)
                      {
                          logFileStreamWriter.Close();
                          logFileFileStream.Close();
                      }
                  });

            Console.WriteLine("Press any key to exit.");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.S)
            {
                cancelationToken.Cancel();
                Console.ReadKey();
            }

        }

        public static void Process(string coonnectionString, Filter filter, string logFile, CancellationToken ct)
        {
            try
            {
                ConnectorBase Connector = ConnectorFactory.CreateConnector(coonnectionString);
                Connector.Connect();

                Package package;

                if (logFile != null && logFile != string.Empty)
                {
                    logFileFileStream = new FileStream(logFile, FileMode.OpenOrCreate);
                    logFileStreamWriter = new StreamWriter(logFileFileStream);
                }
                bool first = true;
                int packagesCount = 0;
                var sw = new Stopwatch();

                while (true)
                {
                    Thread.Sleep(1);
                    while (Connector.Receive(out package))
                    {
                        packagesCount++;
                        if (first) {
                            sw.Start();
                            first = false;
                        }
                        try
                        {
                            bool processMessage = filter.applyFilter(package);
                            if (processMessage)
                            {
                                package.PrintToConsole(packagesCount, ConsoleColor.Green);
                                
                                if (logFileStreamWriter != null)
                                {
                                    logFileStreamWriter.WriteLine("{0}", package.ToString());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(string.Format("SendMessages Exception: {0}", ex.Message));
                            Console.ForegroundColor = ConsoleColor.Green;
                            return;
                        }
                        if (packagesCount == 1000)
                        {
                            sw.Stop();
                            Console.WriteLine("{0} Packages in {1} Milliseconds ~ {2} packages per millisecond and {3} packages per second.", packagesCount, sw.ElapsedMilliseconds, packagesCount / sw.ElapsedMilliseconds, (packagesCount / sw.ElapsedMilliseconds) * 1000);
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Green;
            }

            return;
        }

        #region GUI Methods

        private static void SetWindowPositionAndDimensions(ConsoleMonitorArgs options)
        {
            if (options.Left > 0 || options.Top > 0)
            {
                int xpos = options.Left;
                int ypos = options.Top;
                SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
            }
            if (options.Width > 0 && options.Height > 0)
            {
                Console.SetWindowSize(options.Width, options.Height);
            }
        }

        #endregion
    }

}
