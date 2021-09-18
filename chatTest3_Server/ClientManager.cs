using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Concurrent;

namespace chatTest3_Server
{
    class ClientManager
    {
        public static ConcurrentDictionary<int, ClientData> clientDic = new ConcurrentDictionary<int, ClientData>();
        public event Action<string, string> messageParsingAction = null;
        public event Action<string, int> EventHandler = null;

        public void AddClient(TcpClient newClient)
        {
            ClientData currentClient = new ClientData(newClient);

            try
            {
                currentClient.client.GetStream().BeginRead(currentClient.readBuffer, 0,
                    currentClient.readBuffer.Length, new AsyncCallback(DataReceived),
                    currentClient);
                clientDic.TryAdd(currentClient.clientNumber, currentClient);
            }

            catch (Exception e) { }
        }

        private void DataReceived(IAsyncResult ar)
        {
            ClientData client = ar.AsyncState as ClientData;

            try
            {
                int byteLength = client.client.GetStream().EndRead(ar);
                string strData = Encoding.Default.GetString(client.readBuffer, 0, byteLength);
                client.client.GetStream().BeginRead(client.readBuffer, 0, client.readBuffer.Length,
                    new AsyncCallback(DataReceived), client);
                if (string.IsNullOrEmpty(client.clientName))
                {
                    if (CheckID(strData))
                    {
                        string userName = strData.Substring(3);
                        client.clientName = userName;
                        string accessLog = string.Format("[{0}] {1} Access Server", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), client.clientName);
                        EventHandler.Invoke(accessLog, StaticDefine.ADD_ACCESS_LOG);
                        return;
                    }
                }
            }
            catch (Exception e) { }
        }

        private bool CheckID(string ID)
        {
            if (ID.Contains("%^%"))
                return true;
            return false;
        }
    }
}
