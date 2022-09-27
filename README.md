# TCP_ROM

TCP_ROM is multi process communication library

Enviroment: C#, .NET Standart 2.0

## Features
- Remote Object Mapping (throught Interface)
- Easy to use

## Usage
### Step 1. Define interface for Client
```
    public interface ICalculator : IMpcService
    {
        int Sum(int a, int b);
    }
```

### Step 2. Implement interface for server side
```
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
```
    var port = 8888;
    var serverInstance = new ServerSideCalculator();
    MpcManager.CreateServer<ICalculator>(serverInstance, port);
```

### Step 4. Connect to remote Server
```
    var port = 8888;
    var client = MpcManager.CreateClient<ICalculator>();
    if(!client.Connect("localhost", port))
    {
        // connection is not established
    }
```

### Step 5. Execute remote method
```
    var result = client.Sum(1, 2);
    Console.WriteLine($"Result is: {result}");
```


## Perfomance
10000 executes per second 



## License

Apache 2.0



