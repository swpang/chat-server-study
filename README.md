# chatTestServer_forstudy

<Files>

Program.cs
MainServer.cs
ClientData.cs
ClientManager.cs
StaticDefine.cs

start in Program.cs

1. create new instance of MainServer

2. initialize MainServer
  new ClientManager, chattingLog(ConcurrentBag), accessLog(ConcurrentBag), connection check thread
  run ServerRun()
  
3. create new instance of TcpListener()
  wait and connect clients (and add them to client manager)
  
    Task<TcpClient> acceptTask = listener.AcceptTcpClientAsync();
    acceptTask.Wait();
    TcpClient newClient = acceptTask.Result;
    _clientmanager.AddClient(newClient);
    
4. create new connection check thread (a thread that conducts loops to check connection
  - every 1 sec
  - private void ConnectCheckLoop()
  
  send test message (can be whatever)
  if an exception occurs, remove the client
  
  
//ClientManager.cs
    
addclient
datareceived
checkID

//ClientData.cs

client
readByteData
readBuffer
clientName
clientNumber.
