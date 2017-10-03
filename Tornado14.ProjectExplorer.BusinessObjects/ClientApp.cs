using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.ProjectExplorer.BusinessObjects
{
    public class ClientApp : INotifyPropertyChanged
    {
        private string name;
        private string iPAddress;
        private string status;
        private string programPath;


        public string Name { get { return name; } set { if (name == value) return; name = value; RaisePropertyChanged("Name"); } }
        public string IPAddress { get { return iPAddress; } set { if (iPAddress == value) return; iPAddress = value; RaisePropertyChanged("IPAddress"); } }
        public string Status { get { return status; } set { if (status == value) return; status = value; RaisePropertyChanged("Status"); } }
        public string ProgramPath { get { return programPath; } set { if (programPath == value) return; programPath = value; RaisePropertyChanged("ProgramPath"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
