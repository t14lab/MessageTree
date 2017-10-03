using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tornado14.ProjectExplorer.BusinessObjects
{
    public enum EventMapping
    {
        DataItemBeginChanging_16 = 16,
        DataItemEndChanging_17 = 17,
        CurrentItemChanged_15 = 15,
        DataItemChanged_13 = 13,
        DataItemAdded_18 = 18,
        DataItemDeleted_19 = 19,
    }

    #region Info Events

    [Serializable]
    public class DataItemBeginChanging
    {
        public Guid PK { get; set; }
        public string PropertyName { get; set; }
        public string Type { get; set; }
    }

    [Serializable]
    public class DataItemEndChanging : DataItemBeginChanging { }

    [Serializable]
    public class CurrentItemChanged
    {
        public Guid PK { get; set; }
        public string Type { get; set; }
    }

    #endregion

    [Serializable]
    public class DataItemChanged
    {
        public Guid PK { get; set; }
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
    }

    [Serializable]
    public class DataItemDeleted
    {
        public Guid PK { get; set; }
        public string Type { get; set; }
    }

    [Serializable]
    public class DataItemAdded
    {
        public Guid PK { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
    }

    [Serializable]
    public class GetItem
    {
        public string Type { get; set; }
    }

}
