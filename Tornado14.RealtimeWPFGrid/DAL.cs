using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tornado14.ProjectExplorer.BusinessObjects;
using Tornado14.Utils;

namespace Tornado14.RealtimeWPFGrid
{
    public class DAL
    {
        public ExtendedObservableCollection<Todo> TodoList = new ExtendedObservableCollection<Todo>();
        public ExtendedObservableCollection<Project> ProjectList = new ExtendedObservableCollection<Project>();

        public DAL()
        {
        }
    }
}
