using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Concurrent;

namespace chatTest3_Server
{
    class MainServer
    {
        ClientManager _clientmanager = null;
        ConcurrentBag<string> chattingLog = null;
        ConcurrentBag<string> accessLog = null;
        Thread connectedCheckThread = null;

        public MainServer()
        {
            _clientmanager = new ClientManager();
            chattingLog = new ConcurrentBag<string>();
            accessLog = new ConcurrentBag<string>();
            _clientmanager.EventHandler += ClientEvent;
            _
        }
    } 
}
