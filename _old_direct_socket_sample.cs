using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChromSword.V6.Controllers.Users;
using Common.Logging;

namespace ChromSword.V6.Controllers.Server
{
    public class ServerController
    {

        //https://github.com/jlmessenger/NetSocket
        //https://github.com/umby24/ManagedSockets
        //WCF https://msdn.microsoft.com/en-us/library/cc297274.aspx
        public const int TCP_PORT_BEGIN = 33364;
        public const int MAX_CONNECTIONS_ON_PORT = 10;

        private static SocketPermission _socketPermission;
        private static Socket _socketServer;

        private static Dictionary<Socket, byte[]> _incommingMessages = new Dictionary<Socket, byte[]>();


        public static void Initialize(string ipAddress = null)
        {
            try
            {
                // Resolves a host name to an IPHostEntry instance 
                IPHostEntry ipHost = Dns.GetHostEntry("");
                // Gets first IP address associated with a localhost 
                IPAddress ipAddr = ipHost.AddressList[0];

                if (ipAddress != null)
                {
                    foreach (var ipEach in ipHost.AddressList)
                    {
                        if (ipEach.ToString() == ipAddress)
                        {
                            ipAddr = ipEach;
                            break;
                        }
                    }
                }

                _socketPermission = new SocketPermission(
                    NetworkAccess.Accept,
                    TransportType.Tcp,
                    ipAddr.ToString(),
                    TCP_PORT_BEGIN
                );
                _socketPermission.Demand();


                _socketServer = new Socket(
                        ipAddr.AddressFamily,
                        SocketType.Stream,
                        ProtocolType.Tcp);

                IPAddress serverIP = IPAddress.Any;
                IPEndPoint serverEP = new IPEndPoint(serverIP, TCP_PORT_BEGIN);
                //TODO multiple ports
                _socketServer.Bind(serverEP);
                _socketServer.Listen(MAX_CONNECTIONS_ON_PORT);

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                _socketServer.BeginAccept(aCallback, _socketServer);
            }
            catch (Exception exception)
            {
                LogManager.GetLogger<ServerController>().Error(exception.Message, exception);
            }
        }


        private static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket socketAccept = null;
                // A new Socket to handle remote host communication 
                Socket socketSession = null;

                // Receiving byte array 
                byte[] buffer = new byte[1024];
                // Get Listening Socket object 
                socketAccept = (Socket)ar.AsyncState;
                // Create a new socket 
                socketSession = socketAccept.EndAccept(ar);

                // Using the Nagle algorithm 
                socketSession.NoDelay = false;

                // Creates one object array for passing data 
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = socketSession;

                // Begins to asynchronously receive data 
                socketSession.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    obj            // Specifies infomation for receive operation 
                    );

                //TODO after reiceive? test

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                socketAccept.BeginAccept(aCallback, socketAccept);
            }
            catch (Exception exception)
            {
                LogManager.GetLogger<ServerController>().Error(exception.Message, exception);
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Fetch a user-defined object that contains information 
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array 
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication. 
                var socketSession = (Socket)obj[1];
                
                int bytesRead = socketSession.EndReceive(ar);

                if (bytesRead >= 8)
                {
                    ulong totalSize = BitConverter.ToUInt64(buffer, 0);

                    byte[] bufferFull = null;
                    if (_incommingMessages.ContainsKey(socketSession))
                    {
                        bufferFull = _incommingMessages[socketSession];
                    }
                    else
                    {
                        bufferFull = new byte[totalSize];
                        _incommingMessages.Add(socketSession, bufferFull);
                    }


                    if ((ulong)bytesRead >= totalSize)
                    {
                        //Full message
                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[totalSize];
                        obj[0] = buffernew;
                        obj[1] = socketSession;
                        
                        socketSession.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.GetLogger<ServerController>().Error(exception.Message, exception);
            }
        }

    }
}
