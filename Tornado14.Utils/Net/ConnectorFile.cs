using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils.Net
{
    public class ConnectorFile : ConnectorBase
    {
        string name;
        System.IO.StreamReader file;
        string line;
        Package package = new Package();
        public ConnectorFile(string name)
        {
            this.name = name;
        }
        public override void Connect()
        {
            file = new System.IO.StreamReader(name);
        }
        public override void Close()
        {
            file.Close();
        }
        public override bool Receive(out Package result)
        {
            bool ok = false;
            if ((line = file.ReadLine()) != null)
            {
                // TODO Serialize
                ok = true;
            }
            else
            {
                //Thread.Sleep(100000);
                //throw new Exception("File end reached");
            }
            result = package;
            return ok;
        }
        public bool ReceiveCircular(out Package result)
        {
            bool ok = false;
            if ((line = file.ReadLine()) != null)
            {
                // TODO Deserialize
                ok = true;
            }
            else
            {
                file = new System.IO.StreamReader(name);
            }
            result = package;
            return ok;
        }
    }

}
