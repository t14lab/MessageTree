using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Tornado14.Utils.Net;
using System.ComponentModel;

namespace Tornado14.ProjectExplorer.BusinessObjects
{
    [Serializable]
    public class Project: NetObject, INotifyPropertyChanged, IConvertible 
    {
        private string shortDescription;
        private string description;
        private string filesFolder;
        private string svnRepository;
        private string visualStudioSolution;

        
        
        public string ShortDescription { get { return shortDescription; } set { if (shortDescription == value) return; shortDescription = value; RaisePropertyChanged("ShortDescription"); } }
        public string Description { get { return description; } set { if (description == value) return; description = value; RaisePropertyChanged("Description"); } }
        public string FilesFolder { get { return filesFolder; } set { if (filesFolder == value) return; filesFolder = value; RaisePropertyChanged("FilesFolder"); } }
        public string SVNRepository { get { return svnRepository; } set { if (svnRepository == value) return; svnRepository = value; RaisePropertyChanged("SVNRepository"); } }
        public string VisualStudioSolution { get { return visualStudioSolution; } set { if (visualStudioSolution == value) return; visualStudioSolution = value; RaisePropertyChanged("VisualStudioSolution"); } }

        public override string ToString()
        {
            return string.Format("{0} - {1}",Id, ShortDescription);
        }

        
        public ProjectType Type { get; set; }
        public List<ProjectFeature> FeatureList { get; set; }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static void SetPropValue(object src, string propName, object value)
        {
            src.GetType().GetProperty(propName).SetValue(src, value);
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
