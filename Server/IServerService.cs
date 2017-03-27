using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Server
{
    [ServiceContract]
    interface IServerService
    {
        [OperationContract]
        bool Test(string input);

        [OperationContract]
        void StoreSession(string sessionName);

        [OperationContract]
        string GetSessionName();

        [OperationContract]
        void SendByteData(byte[] data);
    }
}
