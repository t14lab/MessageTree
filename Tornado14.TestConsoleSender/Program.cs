using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tornado14.Utils;
using Tornado14.Utils.Net;

namespace Tornado14.TestConsoleSender
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectorTcp Connector = (ConnectorTcp)ConnectorFactory.CreateConnector("127.0.0.1");
            Connector.Connect();
            Thread.Sleep(1000);
            var r = new Random();
            int count2 = 0;
            int z = 0;
            TestClass t = new TestClass() { Id = 1, Name = "Test" };
            string testData = XmlSerializationHelper.Serialize(t);

            Package testMessage1 = new Package(1, 2, 1, Method.GET, testData);
            Package testMessage2 = new Package(DateTime.Now.AddMilliseconds(z).Ticks, 1, 2, 2, Method.GET, testData);
            Package testMessage3 = new Package(1, 2, 3, Method.GET, testData);
            Package testMessage4 = new Package(1, 2, 4, Method.GET, testData);
            Package testMessage5 = new Package(1, 2, 5, Method.GET, testData);


            bool due = true;
            {
                if (due)
                {

                    Package sendMessage = new Package();
                    for (int count = 0; count < 200000; count++)
                    {
                        count2++;
                        if (count2++ > 9)
                        {
                            count2 = 1;
                        }
                        if (count2 == 7)
                        {
                            z += 70;
                        }

                        if (count2 == 7)
                        {
                            z += 70;
                        }
                        testMessage5.Time = DateTime.Now.AddMilliseconds(z).Ticks;
                        testMessage1.Time = DateTime.Now.Ticks;
                        testMessage2.Time = DateTime.Now.Ticks;
                        testMessage3.Time = DateTime.Now.Ticks;
                        testMessage4.Time = DateTime.Now.Ticks;

                        Connector.Send(testMessage1);
                        Connector.Send(testMessage2);
                        Connector.Send(testMessage3);
                        Connector.Send(testMessage4);
                        Connector.Send(testMessage5);
                        //Thread.Sleep(interval);
                    }
                    Connector.Close();
                }
                due = false;
            }
        }
    }
}
