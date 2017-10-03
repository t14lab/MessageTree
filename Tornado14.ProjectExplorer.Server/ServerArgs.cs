using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.ProjectExplorer.Server
{
    public class ServerArgs
    {

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

        public virtual ServerArgs parseArgs(string[] args)
        {
            ServerArgs options = new ServerArgs();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                return null;
            }
            return options;
        }
    }

}
