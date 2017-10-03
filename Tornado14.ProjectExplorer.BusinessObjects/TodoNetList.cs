using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tornado14.ProjectExplorer.BusinessObjects;
using Tornado14.Utils;
using Tornado14.Utils.Net;

namespace Tornado14.ProjectExplorer.BusinessObjects
{
    public class TodoNetList : TornadoNetList
    {
        private object dataListLock = new object();
        private Dictionary<Guid, Todo> dataList = new Dictionary<Guid, Todo>();

        public Dictionary<Guid, Todo> DataList
        {
            get { return dataList; }
            set { dataList = value; }
        }

        public override void AutoSave()
        {

        }

        public override void SaveData(FileInfo xmlFile)
        {
            lock (dataListLock)
            {
                string data = XmlSerializationHelper.Serialize(dataList.Values.ToArray());
                StreamWriter file = new StreamWriter(xmlFile.FullName);
                file.Write(data);
                file.Close();
            }
        }

        protected override void LoadData(System.IO.FileInfo xmlFile)
        {
            if (xmlFile.Exists)
            {
                StreamReader projectsXmlFileReader = new StreamReader(xmlFile.ToString());
                string projectsInXml = projectsXmlFileReader.ReadToEnd();

                projectsXmlFileReader.Close();
                List<Todo> list = XmlSerializationHelper.Desirialize<List<Todo>>(projectsInXml);
                lock (dataListLock)
                {
                    foreach (Todo obj in list)
                    {
                        dataList.Add(obj.pId, obj);
                    }
                }
                RaiseLogEvent(LogType.Info, string.Format("{0} File found. {1} Entries", xmlFile.ToString(), dataList.Count));
            }
            else
            {
                RaiseLogEvent(LogType.Error, string.Format("{0} File not found.", xmlFile.ToString()));
            }
        }

        protected override void ProcessGET(ConnectorBase Connector, Package package)
        {
            lock (dataListLock)
            {
                foreach (KeyValuePair<Guid, Todo> data in dataList)
                {
                    Package todoPackage = new Package(senderId, package.Sender, 10, Method.Response, XmlSerializationHelper.Serialize(data.Value));
                    Connector.Send(todoPackage);
                }
            }

            //base.ProcessGET(t14Lab.MessageTree.Connector, package);
        }

        protected override void ProcessPUT(ConnectorBase Connector, Package package)
        {
            switch (package.Event)
            {
                case (int)EventMapping.DataItemChanged_13:
                    try
                    {
                        DataItemChanged dataItemChanged = XmlSerializationHelper.Desirialize<DataItemChanged>(package.Data);
                        lock (dataListLock)
                        {
                            Todo.SetPropValue(dataList[dataItemChanged.PK], dataItemChanged.PropertyName, dataItemChanged.Value);
                        }

                    }
                    catch (Exception ex)
                    {
                        RaiseLogEvent(LogType.Error, "TodoNetList.ProcessPUT()" + ex.Message);
                    }
                    break;
                case (int)EventMapping.CurrentItemChanged_15:
                    CurrentItemChanged putParams = XmlSerializationHelper.Desirialize<CurrentItemChanged>(package.Data);
                    if (putParams.Type == typeof(Todo).ToString())
                    {
                        RaiseCurrentItemChangedEvent(putParams.PK);
                    }
                    break;
                default:
                    break;
            }

            //src.GetType().GetProperty(propName).SetValue(src, value);

            //base.ProcessPUT(t14Lab.MessageTree.Connector, package);
        }

        protected override void ProcessPOST(ConnectorBase Connector, Package package)
        {
            try
            {
                DataItemAdded todoAdded = XmlSerializationHelper.Desirialize<DataItemAdded>(package.Data);
                Todo newTodo = new Todo() { pId = todoAdded.PK };
                if (newTodo.pId == null)
                {
                    newTodo.pId = Guid.NewGuid();
                }
                lock (dataListLock)
                {
                    dataList.Add(newTodo.pId, newTodo);
                }
            }
            catch (Exception ex)
            {
                RaiseLogEvent(LogType.Error, "TodoNetList.ProcessPOST()" + ex.Message);
            }
            //base.ProcessPOST(t14Lab.MessageTree.Connector, package);
        }

        protected override void ProcessDELETE(ConnectorBase Connector, Package package)
        {
            try
            {
                DataItemDeleted dataItemDeleted = XmlSerializationHelper.Desirialize<DataItemDeleted>(package.Data);
                lock (dataListLock)
                {
                    dataList.Remove(dataItemDeleted.PK);
                }
            }
            catch (Exception ex)
            {
                RaiseLogEvent(LogType.Error, "TodoNetList.ProcessDELETE()" + ex.Message);
            }

            //base.ProcessDELETE(t14Lab.MessageTree.Connector, package);
        }

    }
}
