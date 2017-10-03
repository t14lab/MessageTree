using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace Tornado14.Utils.Net
{
    [Serializable]
    public class Package : IPackage
    {
        [XmlElement(ElementName = "T")]
        public long Time { get; set; }
        
        [XmlElement(ElementName = "S")]
        public int Sender { get; set; }
        
        [XmlElement(ElementName = "R")]
        public int Recipient { get; set; }
        
        [XmlElement(ElementName = "E")]
        public int Event { get; set; }
        
        [XmlElement(ElementName = "M")]
        public Method Method { get; set; }
        
        [XmlElement(ElementName = "D")]
        public string Data { get; set; }

        public override string ToString()
        {
            // yyyy-MM-dd HH:mm:ss.fff
            return string.Format("{0} - S:{1} R:{2} M:{3} E:{4} D:\r\n{5}\r\n",
                new DateTime(Time).ToString("HH:mm:ss.fff"),
                Sender,
                Recipient,
                Method,
                Event,
                (Data == null) ? string.Empty : Data.ToString()
                );
        }

        public string ToStringFull()
        {
            // yyyy-MM-dd HH:mm:ss.fff
            return string.Format("{0} - S:{1} R:{2} M:{3} E:{4}\r\n--------------------------------------------------------------------------------\r\n{5}\r\n\r\n",
                new DateTime(Time).ToString("HH:mm:ss.fff"),
                Sender,
                Recipient,
                Method,
                Event,
                (Data == null) ? string.Empty : Data.ToString()
                );
        }

        public string ToStringShort()
        {
            // yyyy-MM-dd HH:mm:ss.fff
            return string.Format("{0} - S:{1} R:{2} M:{3} E:{4}",
                new DateTime(Time).ToString("HH:mm:ss.fff"),
                Sender,
                Recipient,
                Method,
                Event,
                (Data == null) ? string.Empty : Data.ToString()
                );
        }

        public Package()
        {
        }

        public Package(int sender, int recipient, int command, Method method, string data)
        {
            this.Time = DateTime.Now.Ticks;
            this.Sender = sender;
            this.Recipient = recipient;
            this.Event = command;
            this.Method = (Method)method;
            this.Data = data;
        }

        public Package(long time, int sender, int recipient, int command, Method method, string data)
        {
            this.Time = time;
            this.Sender = sender;
            this.Recipient = recipient;
            this.Event = command;
            this.Method = (Method)method;
            this.Data = data;
        }

        public void PrintToConsole()
        {
            PrintToConsole(ConsoleColor.White);
        }

        public void PrintToConsole(ConsoleColor standardColor)
        {
            PrintToConsole(standardColor);
        }

        public void PrintToConsole(int packagesCount, ConsoleColor standardColor)
        {
            switch (Method)
            {
                case Method.Response:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case Method.GET:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Method.POST:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case Method.PUT:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case Method.DELETE:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    break;
            }
            Console.WriteLine(string.Format("{0} {1}", packagesCount.ToString().PadRight(7), ToStringFull()));
            Console.ForegroundColor = standardColor;
        }


    }

}
