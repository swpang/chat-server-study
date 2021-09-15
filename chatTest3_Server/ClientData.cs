using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace chatTest3_Server
{
    class ClientData
    {
        public TcpClient client { get; set; }
        public byte[] readByteData { get; set; }
        public int clientNumber;
        public ClientData(TcpClient client)
        {
            this.client = client;
            this.readByteData = new byte[1024];

            string clientEndPoint = client.Client.LocalEndPoint.ToString();
            char[] point = { '.', ':' };
            string[] splittedData = clientEndPoint.Split(point);
            this.clientNumber = int.Parse(splittedData[3]);
            Console.WriteLine("# {0} User Successfully Connected", clientNumber);
        }
    }
}
