using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tornado14.ProjectExplorer.BusinessObjects;
using Tornado14.Utils;

namespace Tornado14.ProjectExplorer.Server
{
    public class DAL
    {
        public ExtendedObservableCollection<ClientApp> ClientAppList = new ExtendedObservableCollection<ClientApp>();

        public DAL()
        {
        }

    }

    
}
