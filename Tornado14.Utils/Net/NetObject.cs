using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils.Net
{
    public class NetObject
    {
        public bool DidNotReadLoL { get; set; }

        private Guid pid;
        public Guid pId { get { return pid; } set { if (pid == value) return; pid = value; RaisePropertyChanged("pId"); } }

        private string id;
        public string Id { get { return id; } set { if (id == value) return; id = value; RaisePropertyChanged("Id"); } }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        
    }



}
