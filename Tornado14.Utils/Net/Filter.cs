using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Net;

namespace Tornado14.Utils.Net
{
    public class Filter : IFilter
    {
        public bool UseOr { get; set; }

        public int[] SenderList { get; set; }
        public int[] RecipientList { get; set; }

        public int[] EventList { get; set; }
        public int[] MethodList { get; set; }

        private bool noFilters = false;

        public bool applyFilter(Package package)
        {
            if (noFilters) return true;
            bool processMessage = true;
            if (UseOr)
            {
                processMessage = false;
            }
            Filter filter = this;
            Dictionary<int[], int> values = new Dictionary<int[], int>();
            if (SenderList != null) values.Add(SenderList, package.Sender);
            if (RecipientList != null) values.Add(RecipientList, package.Recipient);
            if (EventList != null) values.Add(EventList, package.Event);
            if (MethodList != null) values.Add(MethodList, (int)package.Method);
            foreach (KeyValuePair<int[], int> value in values)
            {
                processMessage = CompareValues(value.Key, value.Value);
                if (UseOr)
                {
                    if (processMessage) return processMessage;
                }
                else
                {
                    if (!processMessage) return processMessage;
                }
            }

            return processMessage;
        }

        private static bool CompareValues(int[] values, int value, bool initValue = false)
        {
            bool equals = initValue;
            if (values != null)
            {
                foreach (int param in values)
                {
                    if (value == param)
                    {
                        equals = true;
                        break;
                    }
                }
            }
            return equals;
        }


        private void setDefaultValues()
        {

        }

        private bool allFiltersNull()
        {
            if (this.SenderList == null &&
                this.RecipientList == null &&
                this.EventList == null &&
                this.MethodList == null) 
                return true;
            return false;
        }

        public static string[] ParseCommaSeparatedStrings(string paramString)
        {
            try
            {
                string[] parameters = paramString.Split(',');
                List<string> intParams = new List<string>();
                foreach (string param in parameters)
                {
                    intParams.Add(param);
                }
                return intParams.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("ParseCommaSeparatedStrings Exception: {0}", ex.Message));
                return null;
            }
        }

        public static int[] ParseCommaSeparatedIntegers(string paramString)
        {
            try
            {
                string[] parameters = paramString.Split(',');
                List<int> intParams = new List<int>();
                foreach (string param in parameters)
                {

                    intParams.Add(int.Parse(param));
                }
                return intParams.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("ParseCommaSeparatedStrings Exception: {0}", ex.Message));
                return null;
            }
        }

        public static Method[] ParseCommaSeparatedMethods(string paramString)
        {
            try
            {
                string[] parameters = paramString.Split(',');
                List<Method> intParams = new List<Method>();
                foreach (string param in parameters)
                {

                    intParams.Add((Method)Enum.Parse(typeof(Method), param));
                }
                return intParams.ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("ParseCommaSeparatedStrings Exception: {0}", ex.Message));
                return null;
            }
        }

    }
}
