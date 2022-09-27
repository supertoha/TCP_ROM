# TCP_ROM

TCP_ROM is multi process communication library

Enviroment: C#, .NET Standart 2.0

## Features
- Remote Object Mapping (throught Interface)
- Easy to use

## Usage
### Step 1. Define the same interface for Client and Server
```csharp
    using MultiProcessCommunicator;
    public interface ICalculator : IMpcService
    {
        int Sum(int a, int b);
    }
```

### Step 2. Implement interface for the Server side
```csharp
    public class ServerSideCalculator : ICalculator
    {
        public bool Connect(string host, int port){return true;}

        public void Disconnect(){}

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
```
### Step 3. Run Server
```csharp
    var port = 8888;
    var serverInstance = new ServerSideCalculator();
    MpcManager.CreateServer<ICalculator>(serverInstance, port);
```

### Step 4. Connect to remote Server
```csharp
    var port = 8888;
    var host = "localhost";
    var client = MpcManager.CreateClient<ICalculator>();
    if(!client.Connect(host, port))
    {
        // connection is not established
    }
```

### Step 5. Execute remote method
```csharp
    var result = client.Sum(1, 2);
    Console.WriteLine($"Result is: {result}");
```
```mermaid
sequenceDiagram
Client ->> Server: Connect
Server-->>Client: connected: True

Client ->> Server: Sum(1,2)
Server-->>Client: result: 3
```


## Perfomance
~10000 executes per second 



## License

Apache 2.0



