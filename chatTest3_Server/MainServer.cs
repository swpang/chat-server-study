using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
            _clientmanager.messageParsingAction += MessageParsing;
            Task serverStart = Task.Run(() =>
            {
                ServerRun();
            });
            connectedCheckThread = new Thread(ConnectCheckLoop);
            connectedCheckThread.Start();
        }

        private void ConnectCheckLoop()
        {
            while (true)
            {
                foreach (var item in ClientManager.clientDic)
                {
                    try
                    {
                        string sendStringData = "Admin<TEST>";
                        byte[] sendByteData = new byte[sendStringData.Length];
                        sendByteData = Encoding.Default.GetBytes(sendStringData);
                        item.Value.client.GetStream().Write(sendByteData, 0, sendByteData.Length);
                    }
                    catch (Exception e)
                    {
                        RemoveClient(item.Value);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void RemoveClient (ClientData targetClient)
        {
            ClientData result = null;
            ClientManager.clientDic.TryRemove(targetClient.clientNumber, out result);
            string leaveLog = string.Format("[{0}] {1} Leave Server", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                result.clientName);
            accessLog.Add(leaveLog);
        }

        private void MessageParsing (string sender, string message)
        {
            List<string> msgList = new List<string>();

            string[] msgArray = message.Split('>');
            foreach (var item in msgArray)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                msgList.Add(item);
            }
            SendMsgToClient(msgList, sender);
        }

        private void SendMsgToClient(List<string> msgList, string sender)
        {
            string LogMessage = "";
            string parsedMessage = "";
            string receiver = "";

            int senderNumber = -1;
            int receiverNumber = -1;

            foreach (var item in msgList)
            {
                string[] splittedMsg = item.Split('<');

                receiver = splittedMsg[0];
                parsedMessage = string.Format("{0}<{1}>", sender, splittedMsg[1]);

                senderNumber = GetClientNumber(sender);
                receiverNumber = GetClientNumber(receiver);

                if (senderNumber == -1 || receiverNumber == -1)
                    return;
                if (parsedMessage.Contains("<GiveMeUserList>"))
                {
                    string userListStringData = "Admin<";
                    foreach (var el in ClientManager.clientDic)
                        userListStringData += string.Format("${0}", el.Value.clientName);
                    userListStringData += ">";
                    byte[] userListByteData = new byte[userListStringData.Length];
                    userListByteData = Encoding.Default.GetBytes(userListStringData);
                    ClientManager.clientDic[receiverNumber].client.GetStream().Write(userListByteData, 0,
                        userListByteData.Length);
                    return;
                }

                LogMessage = string.Format(@"[{0}] [{1}] -> [{2}], {3}", DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"),
                    sender, receiver, splittedMsg[1]);
                ClientEvent(LogMessage, StaticDefine.ADD_CHATTING_LOG);
                byte[] sendByteData = Encoding.Default.GetBytes(parsedMessage);
                ClientManager.clientDic[receiverNumber].client.GetStream().Write(sendByteData, 0,
                    sendByteData.Length);
            }
        }
        private int GetClientNumber(string targetClientName)
        {
            foreach (var item in ClientManager.clientDic)
            {
                if (item.Value.clientName == targetClientName)
                    return item.Value.clientNumber;
            }
            return -1;
        }
        private void ClientEvent(string message, int key)
        {
            switch (key)
            {
                case StaticDefine.ADD_ACCESS_LOG:
                    accessLog.Add(message);
                    break;
                case StaticDefine.ADD_CHATTING_LOG:
                    chattingLog.Add(message);
                    break;
            }
        }

        private void ServerRun()
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 9999));
            listener.Start();

            while(true)
            {
                Task<TcpClient> acceptTask = listener.AcceptTcpClientAsync();
                acceptTask.Wait();
                TcpClient newClient = acceptTask.Result;
                _clientmanager.AddClient(newClient);
            }
        }

        public void ConsoleView()
        {
            while(true)
            {
                Console.WriteLine("=============Server=============");
                Console.WriteLine("1. 현재 접속 인원 확인");
                Console.WriteLine("2. 접속 기록 확인");
                Console.WriteLine("3. 채팅 로그 확인");
                Console.WriteLine("0. 종료");
                Console.WriteLine("================================");

                string key = Console.ReadLine();
                int order = 0;

                if (int.TryParse(key, out order))
                {
                    switch (order)
                    {
                        case StaticDefine.SHOW_CURRENT_CLIENT:
                            ShowCurrentClient();
                            break;
                        case StaticDefine.SHOW_ACCESS_LOG:
                            ShowAccessLog();
                            break;
                        case StaticDefine.SHOW_CHATTING_LOG:
                            ShowChattingLog();
                            break;
                        case StaticDefine.EXIT:
                            connectedCheckThread.Abort();
                            return;
                        default:
                            Console.WriteLine("Wrong input");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Wrong input");
                    Console.ReadKey();
                }
            }
            Console.Clear();
            Thread.Sleep(50);
        }

        private void ShowChattingLog()
        {
            if (chattingLog.Count == 0)
            {
                Console.WriteLine("No records available");
                Console.ReadKey();
                return;
            }
            foreach (var item in chattingLog)
                Console.WriteLine(item);
            Console.ReadKey();
        }

        private void ShowAccessLog()
        {
            if (accessLog.Count == 0)
            {
                Console.WriteLine("No records available");
                Console.ReadKey();
                return;
            }
            foreach (var item in accessLog)
                Console.WriteLine(item);
            Console.ReadKey();
        }

        private void ShowCurrentClient()
        {
            if (ClientManager.clientDic.Count == 0)
            {
                Console.WriteLine("No clients");
                Console.ReadKey();
                return;
            }
            foreach (var item in ClientManager.clientDic)
                Console.WriteLine(item.Value.clientName);
            Console.ReadKey();
        }
    } 
}
