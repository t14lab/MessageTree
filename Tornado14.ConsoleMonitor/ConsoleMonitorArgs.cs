using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.ConsoleMonitor
{
    public class ConsoleMonitorArgs
    {
        [Option('c', "connectionString", Required = true, HelpText = "Connection string")]
        public string ConnectionString { get; set; }

        [Option('o', "useOr", Required = false, HelpText = "Use OR between filters")]
        public bool UseOr { get; set; }

        [Option('s', "sender", Required = false, HelpText = "Filter - Sender")]
        public string Sender { get; set; }

        [Option('r', "recipient", Required = false, HelpText = "Filter - Recipient")]
        public string Recipient { get; set; }

        [Option('e', "event", Required = false, HelpText = "Filter - Event")]
        public string Event { get; set; }

        [Option('m', "Method", Required = false, HelpText = "Filter - Method")]
        public string Method { get; set; }



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


        public static ConsoleMonitorArgs ParseArgs(string[] args)
        {
            ConsoleMonitorArgs options = new ConsoleMonitorArgs();
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
            Console.WriteLine(string.Format("Use OR between filters: '{0}'", options.UseOr));
            Console.WriteLine(string.Format("Sender: '{0}'", options.Sender));
            Console.WriteLine(string.Format("Recipient: '{0}'", options.Recipient));
            Console.WriteLine(string.Format("Event: '{0}'", options.Event));
            Console.WriteLine(string.Format("Method: '{0}'", options.Method));
            Console.WriteLine(string.Format("LogFile: '{0}'", options.LogFile));
            Console.WriteLine(string.Format("[Position] Top: '{0}' Left: '{1}'", options.Top, options.Left));
            Console.WriteLine(string.Format("[Dimensions] Width: '{0}' Height:'{1}'", options.Width, options.Height));
            Console.ResetColor();

            return options;
        }
       
    }
}
