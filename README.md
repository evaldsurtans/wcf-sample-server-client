# wcf-sample-server-client
C# based WCF server client sample application (better alternative for .net to TCP sockets etc)

Server app must be "Run as administrator" or when testing via Visual Studio also it must be "Run as administrator"

Server interface + Class
```
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
```

Client is generated/updated automatically using Visual Studio:
```
//To add ServerServiceClient use
//svcutil.exe http://localhost:8000/ServerService?wsdl
//OR easier way 
//To add new right click on project "Add Service Reference"
//Update right click on "Service References->ServiceReference1" then "Update Service Reference"
ServerServiceClient client = new ServerServiceClient();
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
```