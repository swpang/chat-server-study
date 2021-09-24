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
        public Byte[] readBuffer { get; set; }
        public StringBuilder currentMsg { get; set; }
        public string clientName { get; set; }
        public int clientNumber { get; set; }
        public ClientData(TcpClient client)
        {
            this.client = client;

            currentMsg = new StringBuilder();
            readBuffer = new byte[1024];

            char[] splitDivision = new char[2];
            splitDivision[0] = '.';
            splitDivision[1] = ':';

            string[] temp = null;

            temp = client.Client.LocalEndPoint.ToString().Split(splitDivision);
            //Socket.LocalEndPoint? IP Address + Port Number
            this.clientNumber = int.Parse(temp[3]);
        }
    }
}
