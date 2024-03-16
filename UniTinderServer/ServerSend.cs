using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniTinderServer
{
    class ServerSend
    {

        private static void SendTCPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].tcp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxUsers; i++)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i < Server.MaxUsers; i++)
            {
                if (i != exceptClient)
                {
                    Server.clients[i].tcp.SendData(packet);
                }
                
            }
        }

        public static void SendIntoApp(int toClient, int IDInDatabase, int userCount, bool check = true)
        {
            using (Packet packet = new Packet((int)ServerPackets.sendIntoApp)) 
            {
                packet.Write(check);
                packet.Write(userCount);
                packet.Write(IDInDatabase);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        public static void Welcome(int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        public static void receiveMessageFromUser(int fromClient,int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.receiveMessageFromUser))
            {   
                packet.Write(fromClient);
                packet.Write(message);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

    }
}
