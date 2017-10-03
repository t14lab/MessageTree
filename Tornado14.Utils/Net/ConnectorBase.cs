using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using System.IO;
using System.Threading;
using log4net;

namespace Tornado14.Utils.Net
{

    public class ConnectorBase
    {
        public virtual void Connect() { Console.WriteLine("Connect"); }
        public virtual void Close() { Console.WriteLine("Close"); }
        public virtual bool Receive(out Package package)
        {
            package = new Package();
            Console.WriteLine("Recv");
            return false;
        }

        public virtual bool Send(Package package) { /*Console.WriteLine("Send")*/; return false; }
    }
}
