using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tornado14.ProjectExplorer.BusinessObjects;
using Tornado14.ProjectExplorer.Data.Properties;
using Tornado14.Utils;
using Tornado14.Utils.Net;


namespace Tornado14.ProjectExplorer.Data
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

        private static FileInfo taskFile;
        private static FileInfo projectFile;
        private static TodoNetList taskNetList;
        private static ProjectNetList projectNetList;

        static void Main(string[] args)
        {
            DataArgs options = DataArgs.ParseArgs(args);
            try
            {
                SetWindowPositionAndDimensions(options);
                string[] files = options.DataFile.Split(',');
                string[] senderIds = options.SenderId.Split(',');

                int taskId = int.Parse(senderIds[0]);
                taskFile = new FileInfo(files[0]);
                int projectId = int.Parse(senderIds[1]);
                projectFile = new FileInfo(files[1]);

                taskNetList = new TodoNetList();
                projectNetList = new ProjectNetList();
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

            }

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Start Exception: {0}", ex.Message));
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            
            Console.WriteLine("Press any key to exit.");
            ConsoleKeyInfo key = Console.ReadKey();
            if (key.Key == ConsoleKey.S)
            {
                Console.ReadKey();
            }

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


        #region GUI Methods

        private static void SetWindowPositionAndDimensions(DataArgs options)
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
