using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UniTinderServer
{
    class Server
    {  
        
        public static int MaxUsers { get; private set; } 
        public static int Port { get; private set; }
        public static Dictionary<int,Client> clients = new Dictionary<int,Client>();
        private static TcpListener _tcpListener;
        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static DataBase dataBase;

        public static void Start(int maxUsers, int port)
        {
            string path = "Data Source = \"C:\\UniTinderWithData.db\"";
            dataBase = new DataBase(path);

            MaxUsers = maxUsers;
            Port = port;

            Console.WriteLine("Starting server");
            InitializeServerData();

            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on {Port}."); 
        }

        private static void TCPConnectCallback(IAsyncResult result) // асинхронный колбэк
        {
            TcpClient client = _tcpListener.EndAcceptTcpClient(result); // завершение приема и получение клиента
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); // прием нового клиента

            Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxUsers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client); // для клиента вызываем Connect(client) для сохранения его сокета в словаре
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server is full!");

        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxUsers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.connectUser, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.sendMessageToServer, ServerHandle.SendMessageToServer },
                { (int)ClientPackets.registerNewUser, ServerHandle.RegisteredNewUser },
            };
            Console.WriteLine("Initialized packets.");
        }

    }
}
