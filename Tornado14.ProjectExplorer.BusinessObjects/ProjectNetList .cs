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
    public class ProjectNetList : TornadoNetList
    {
        private object dataListLock = new object();
        private Dictionary<Guid, Project> dataList = new Dictionary<Guid, Project>();

        public Dictionary<Guid, Project> DataList
        {
            get { return dataList; }
            set { dataList = value; }
        }

        private FileInfo taskFile;

        public override void AutoSave()
        {
            FileInfo autosaveFileTodos = new FileInfo(string.Format("{0}\\{1}_Autosave{2}", taskFile.Directory, taskFile.Name.Replace(taskFile.Extension, string.Empty), taskFile.Extension));
            SaveData(autosaveFileTodos);
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
                taskFile = xmlFile;
                StreamReader projectsXmlFileReader = new StreamReader(xmlFile.ToString());
                string projectsInXml = projectsXmlFileReader.ReadToEnd();

                projectsXmlFileReader.Close();
                List<Project> list = XmlSerializationHelper.Desirialize<List<Project>>(projectsInXml);
                lock (dataListLock)
                {
                    foreach (Project obj in list)
                    {
                        dataList.Add(obj.pId, obj);
                    }
                }

                Console.WriteLine("{0} File found. {1} Entries", xmlFile.ToString(), dataList.Count);
            }
            else
            {
                Console.WriteLine("todos.xml File not found.");
            }
        }

        protected override void ProcessGET(ConnectorBase Connector, Package package)
        {
            lock (dataListLock)
            {
                foreach (KeyValuePair<Guid, Project> data in dataList)
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
                        RaiseLogEvent(LogType.Error, "ProjectNetList.ProcessPUT()" + ex.Message);
                    }
                    break;
                case (int)EventMapping.CurrentItemChanged_15:
                    CurrentItemChanged putParams = XmlSerializationHelper.Desirialize<CurrentItemChanged>(package.Data);
                    if (putParams.Type == typeof(Project).ToString())
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
                DataItemAdded projectAdded = XmlSerializationHelper.Desirialize<DataItemAdded>(package.Data);
                Project newProject = new Project() { pId = projectAdded.PK };
                lock (dataListLock)
                {
                    dataList.Add(newProject.pId, newProject);
                }
            }
            catch (Exception ex)
            {
                RaiseLogEvent(LogType.Error, "ProjectNetList.ProcessPOST()" + ex.Message);
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
                RaiseLogEvent(LogType.Error, "ProjectNetList.ProcessDELETE()" + ex.Message);

            }

            //base.ProcessDELETE(t14Lab.MessageTree.Connector, package);
        }

    }
}
