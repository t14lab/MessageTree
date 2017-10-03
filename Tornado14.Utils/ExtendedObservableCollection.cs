using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.Utils
{
    /// <summary>
    /// Extends the ObservableCollection with the possibility to react on INotifyPropertyChanged.PropertyChanged
    /// of an element inside the collection.
    /// </summary>
    /// <typeparam name="T">A type implementing INotifyPropertyChanged.</typeparam>
    public class ExtendedObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {

        /// <summary>
        /// Occurs when the PropertyChanged of an element inside the collection has changed.
        /// </summary>
        public event PropertyChangedEventHandler ElementContentChanged;

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add &&
                e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ElementContentChanged != null)
                ElementContentChanged(sender, e);
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

    }
}
