using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Tornado14.Utils;
using System.ComponentModel;
using System.Xml.Serialization;
using Tornado14.Utils.Net;


namespace Tornado14.ProjectExplorer.BusinessObjects
{
    [Serializable]
    public class Todo : NetObject, INotifyPropertyChanged, IConvertible 
    {
        private string shortDescription;
        private string description;
        private string result;
        private string publicText;
        private string currentState;

        private Status status;
        private int progress;

        private string filesFolder;

        private Guid projectPid;
        private Guid sprintPid;
        private List<Guid> features;

        

        public string ShortDescription { get { return shortDescription; } set { if (shortDescription == value) return; shortDescription = value; RaisePropertyChanged("ShortDescription"); } }
        public string Description { get { return description; } set { if (description == value) return; description = value; RaisePropertyChanged("Description"); } }
        public string Result { get { return result; } set { if (result == value) return; result = value; RaisePropertyChanged("Result"); } }
        public string PublicText { get { return publicText; } set { if (publicText == value) return; publicText = value; RaisePropertyChanged("PublicText"); } }
        public string CurrentState { get { return currentState; } set { if (currentState == value) return; currentState = value; RaisePropertyChanged("CurrentState"); } }
        
        public Status Status { get { return status; } set { if (status == value) return; status = value; RaisePropertyChanged("Status"); } }
        public int Progress { get { return progress; } set { if (progress == value) return; progress = value; RaisePropertyChanged("Progress"); } }
        
        public string FilesFolder { get { return filesFolder; } set { if (filesFolder == value) return; filesFolder = value; RaisePropertyChanged("FilesFolder"); } }

        public Guid ProjectPid { get { return projectPid; } set { if (projectPid == value) return; projectPid = value; RaisePropertyChanged("ProjectPid"); } }
        public Guid SprintPid { get { return sprintPid; } set { if (sprintPid == value) return; sprintPid = value; RaisePropertyChanged("SprintPid"); } }
        //public List<Guid> AlsoForProjects { get { return id; } set; }
        public List<Guid> Features { get { return features; } set { if (features == value) return; features = value; RaisePropertyChanged("Features"); } }


        public bool OpenFilesFolder(string dataFolder)
        {
            bool folderCreated = false;
            string taskFolder = FileHelper.RemoveBadCharactersFromFileName(string.Format("{0} {1}", Id, ShortDescription));
            DirectoryInfo tasksFolder = new DirectoryInfo(Path.Combine(dataFolder, @"ProjectExplorer\Files\Tasks\", taskFolder));
            if (!tasksFolder.Exists)
            {
                tasksFolder.Create();
                FilesFolder = tasksFolder.ToString();
                folderCreated = true;
            }
            Process.Start("explorer", tasksFolder.ToString());
            return folderCreated;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetPropValue(object src, string propName, object value)
        {
            src.GetType().GetProperty(propName).SetValue(src, value);
        }


        public static void CreateTestData(int maxTodosCount, string fileName)
        {

            List<Todo> todoList = new List<Todo>();
            using (StreamWriter outfile = new StreamWriter(fileName))
            {
                int count = 1;
                int count2 = 1;
                Guid projectPid = Guid.NewGuid();
                for (int a = 0; a < maxTodosCount; a++)
                {
                    Status status = Status.Test;
                    if (count == 1)
                    {
                        status = Status.Done;
                    }
                    else if (count == 2)
                    {
                        status = Status.Progress;
                    }
                    else if (count == 3)
                    {
                        status = Status.Today;
                    }
                    else
                    {
                        status = Status.Backlog;
                    }
                    if (count == 7)
                    {
                        count = 1;
                    }
                    else
                    {
                        count++;
                    }
                    if (count2 == 200)
                    {
                        projectPid = Guid.NewGuid();
                        count2 = 1;
                    }
                    count2++;
                    Todo todo = new Todo()
                    {
                        pId = Guid.NewGuid(),
                        Id = "T14T-" + a.ToString().PadLeft(6, '0'),
                        ShortDescription = "Test Todo " + a,
                        CurrentState = "Current state " + a,
                        Description = "Description" + a,
                        Result = "Result" + a,
                        Status = status,
                        ProjectPid = projectPid
                    };

                    outfile.WriteLine(XmlSerializationHelper.Serialize(todo));
                }
            }
        }

        #region IConvertable 

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            NetObject no = new NetObject();
            no.pId = pId;
            return no;
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
