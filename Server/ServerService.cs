using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class ServerService : IServerService
    {
        private string _sessionName = "";

        public void StoreSession(string sessionName)
        {
            _sessionName = sessionName;
        }

        public string GetSessionName()
        {
            return _sessionName;
        }

        public void SendByteData(byte[] data)
        {
            Console.WriteLine("Received: {0}", data.Length);
        }

        public bool Test(string input)
        {
            Console.WriteLine("Test: {0}", input);
            return true;
        }
    }
}
