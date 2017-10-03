using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.ProjectExplorer.Data
{
    public class DataArgs
    {
        [Option('c', "connectionString", Required = true, HelpText = "Connection string")]
        public string ConnectionString { get; set; }

        [Option('d', "dataFile", Required = false, HelpText = "Data XML File or files, comma separated")]
        public string DataFile { get; set; }

        [Option('s', "senderId", Required = false, HelpText = "Sender ID")]
        public string SenderId { get; set; }

        [Option('l', "logFile", Required = false, HelpText = "Log file full path")]
        public string LogFile { get; set; }

        [Option('y', "top", Required = false, HelpText = "Console window screen position top")]
        public int Top { get; set; }

        [Option('x', "left", Required = false, HelpText = "Console window screen position left")]
        public int Left { get; set; }

        [Option('w', "width", Required = false, HelpText = "Console window width")]
        public int Width { get; set; }

        [Option('h', "height", Required = false, HelpText = "Console window height")]
        public int Height { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }


        public static DataArgs ParseArgs(string[] args)
        {
            DataArgs options = new DataArgs();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                Console.WriteLine("Wrong params. Press any key to close.");
                foreach (string arg in args)
                {
                    Console.WriteLine(arg);
                }
                Console.ReadKey();
                Environment.Exit(100);
            }

            // Show results
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(string.Format("Connection string: '{0}'", options.ConnectionString));
            Console.WriteLine("Filters:");
            Console.WriteLine(string.Format("LogFile: '{0}'", options.LogFile));
            Console.WriteLine(string.Format("[Position] Top: '{0}' Left: '{1}'", options.Top, options.Left));
            Console.WriteLine(string.Format("[Dimensions] Width: '{0}' Height:'{1}'", options.Width, options.Height));
            Console.ResetColor();

            return options;
        }
       
    }
}
