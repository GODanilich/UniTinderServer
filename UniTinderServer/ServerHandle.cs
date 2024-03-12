using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniTinderServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now user {fromClient} and has username {username}");
            if (fromClient != clientCheck)
            {
                Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientCheck})!");
            }
            // TODO: send user into app
        }

        public static void SendMessageToServer(int fromClient, Packet packet)
        {
            int clientCheck = packet.ReadInt();
            string message = packet.ReadString();

            Console.WriteLine($" message from {Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} id = {fromClient}: {message}");
            if (fromClient != clientCheck)
            {
                Console.WriteLine($"Player \"\" (ID: {fromClient}) has assumed the wrong client ID ({clientCheck})!");
            }
            // TODO: send user into app
        }
    }
}
