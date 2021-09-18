using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace chatTest3_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // MyServer a = new MyServer();
            MainServer a = new MainServer();
            a.ConsoleView();
        }
    }
}
