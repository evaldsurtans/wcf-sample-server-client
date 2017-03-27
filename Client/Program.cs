using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.ServiceReference1;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //To add ServerServiceClient use
            //svcutil.exe http://localhost:8000/ServerService?wsdl
            //OR easier way 
            //To add new right click on project "Add Service Reference"
            //Update right click on "Service References->ServiceReference1" then "Update Service Reference"
            ServerServiceClient client = new ServerServiceClient("BasicHttpBinding_IServerService", "http://localhost:8000/ServerService");
            var result = client.Test("test text");

            Console.WriteLine("Session Name:");
            var sessionName = Console.ReadLine();
            client.StoreSession(sessionName);
            Console.WriteLine(client.GetSessionName());
            Console.ReadLine();

            client.SendByteData(new byte[100000]);
            Console.WriteLine("Uploaded");

            Console.ReadLine();
            client.Close();
        }
    }
}
