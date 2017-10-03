using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils.Net
{
    public class LogEventArgs
    {
        public String Text { get; private set; }
        public LogType Type { get; private set; }
        public LogEventArgs(LogType type, string text)
        {
            Text = text;
            Type = type;
        }
    }
}
