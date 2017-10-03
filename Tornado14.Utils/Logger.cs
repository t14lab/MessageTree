using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils
{
    public class Logger
    {
        private PatternLayout _layout = new PatternLayout();
        private const string LOG_PATTERN = "%d [%t] %-5p %m%n";

        public string DefaultPattern
        {
            get { return LOG_PATTERN; }
        }

        public Logger()
        {
            _layout.ConversionPattern = DefaultPattern;
            _layout.ActivateOptions();
        }

        public PatternLayout DefaultLayout
        {
            get { return _layout; }
        }

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy =
                (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(appender);
        }

        static Logger()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LOG_PATTERN;
            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            tracer.Name = "tracer1";
            
            hierarchy.Root.AddAppender(tracer);

            FileAppender roller = new FileAppender();
            roller.Layout = patternLayout;
            roller.AppendToFile = false;
            roller.File = "dnservices1.txt";
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        public static ILog Create()
        {
            return LogManager.GetLogger("tracer12");
        }
    }
}