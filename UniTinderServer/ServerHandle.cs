using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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


            //int dbID = Server.dataBase.GetUserIDByNickname(username);
            int dbID = Convert.ToInt32(Server.dataBase.FindValueInColumn("User", "Nickname", username, "UserID"));

            if (dbID != 0)
            {   
                if (!Server.ConnectedUsersIDList.Contains(dbID)) 
                {
                    
                    Server.clients[fromClient].tcp.idInDatabase = dbID;
                    Server.ConnectedUsersIDList.Add(dbID);
                    ServerSend.SendIntoApp(fromClient, dbID);

                    Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now user {fromClient} and has username {username}");
                }
                else
                {
                    Console.WriteLine($"User {username} is already connected");
                }
            }
            else
            {
                Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} username {username} is not registered");
            }

            
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

        public static void RegisteredNewUser(int fromClient, Packet packet)
        {
            int clientCheck = packet.ReadInt();
            string message = packet.ReadString();

            Console.WriteLine(message);

            int dbID = Convert.ToInt32(Server.dataBase.FindValueInColumn("User", "Nickname", message, "UserID"));

            if (dbID == 0)
            {
                Server.dataBase.InsertRow("User", new string[] { "Nickname" }, new string[] { message });

                Server.clients[fromClient].tcp.idInDatabase = dbID;
                Server.ConnectedUsersIDList.Add(dbID);
                ServerSend.SendIntoApp(fromClient, dbID);

                Console.WriteLine($"Created user {message} and logged in");
            }
            else { Console.WriteLine("User is already exists"); }




            //int dbID = Server.dataBase.GetUserIDByNickname(message);







            if (fromClient != clientCheck)
            {

                Console.WriteLine($"Player \"\" (ID: {fromClient}) has assumed the wrong client ID ({clientCheck})!");
            }
            // TODO: send user into app
        }
    }
}
