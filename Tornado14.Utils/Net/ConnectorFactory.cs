using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils.Net
{
    public class ConnectorFactory
    {
        static public ConnectorBase CreateConnector(string connection)
        {
            IPAddress address;
            if (IPAddress.TryParse(connection, out address))
            {
                return new ConnectorTcp(address);
            }
            else if (connection.EndsWith(".txt"))
            {
                return new ConnectorFile(connection);
            }
            throw new Exception("No connection defined");
        }
    }
}
