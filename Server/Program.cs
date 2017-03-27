using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Need to run as administrator
            ServiceHost host = new ServiceHost(typeof(ServerService), new Uri("http://localhost:8000/ServerService"));

            host.Open();
            Console.WriteLine("The service is ready.");
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.WriteLine();
            Console.ReadLine();
                
            host.Close();
        }
    }
}
