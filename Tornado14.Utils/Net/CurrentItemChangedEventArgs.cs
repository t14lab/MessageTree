using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tornado14.Utils.Net
{
    public class CurrentItemChangedEventArgs
    {
        public Guid PK { get; private set; }
        public CurrentItemChangedEventArgs(Guid pk)
        {
            PK = pk;
        }
    }
}
