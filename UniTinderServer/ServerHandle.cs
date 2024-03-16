using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UniTinderServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientCheck = packet.ReadInt();
            string email = packet.ReadString();
            string password = packet.ReadString();


            //int dbID = Server.dataBase.GetUserIDByNickname(username);
            int dbID = Convert.ToInt32(Server.dataBase.FindValueInColumn("User", "Email", email, "UserID"));

            if (dbID != 0)
            {
                string dbpassword = Server.dataBase.FindValueInColumn("User", "UserID", Convert.ToString(dbID), "Nickname");

                if (dbpassword == password)
                {
                    if (!Server.ConnectedUsersIDList.Contains(dbID))
                    {

                        Server.clients[fromClient].tcp.idInDatabase = dbID;
                        Server.ConnectedUsersIDList.Add(dbID);
                        ServerSend.SendIntoApp(fromClient, dbID, Server.ConnectedUsersIDList.Count());

                        Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully and is now user {fromClient} and has username {email}");
                    }
                    else
                    {
                        Console.WriteLine($"User {email} is already connected");
                    }
                }
            else
                {
                    Console.WriteLine("Incorrect password");
                    ServerSend.SendIntoApp(fromClient, dbID, Server.ConnectedUsersIDList.Count(), false);
                }

                
            }
            else
            {
                Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} username {email} is not registered");
            }

            
            if (fromClient != clientCheck)
            {
                Console.WriteLine($"Player \"{email}\" (ID: {fromClient}) has assumed the wrong client ID ({clientCheck})!");
            }
            // TODO: send user into app
        }

        public static void SendMessageToUser(int fromClient, Packet packet)
        {
            int clientCheck = packet.ReadInt();
            int toUserID = packet.ReadInt();
            string message = packet.ReadString();

            ServerSend.receiveMessageFromUser(fromClient, toUserID, message);
            Console.WriteLine($" message from {Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} id = {fromClient}: {message} to {toUserID}");
            if (fromClient != clientCheck)
            {
                Console.WriteLine($"Player \"\" (ID: {fromClient}) has assumed the wrong client ID ({clientCheck})!");
            }
            // TODO: send user into app
        }

        public static void RegisteredNewUser(int fromClient, Packet packet)
        {
            int clientCheck = packet.ReadInt();
            string nickname = packet.ReadString();
            string email = packet.ReadString();
            string city = packet.ReadString();
            string job = packet.ReadString();
            int experienceTime = packet.ReadInt();

            //Console.WriteLine(message);

            int dbID = Convert.ToInt32(Server.dataBase.FindValueInColumn("User", "Email", email, "UserID"));

            if (dbID == 0)
            {
                Server.dataBase.InsertRow("User", new string[] { "Nickname", "Email", "Address", "Work", "WorkAge" },
                    new string[] { nickname, email, city, job, Convert.ToString(experienceTime)});

                Server.clients[fromClient].tcp.idInDatabase = dbID;
                Server.ConnectedUsersIDList.Add(dbID);
                ServerSend.SendIntoApp(fromClient, dbID, Server.ConnectedUsersIDList.Count());

                Console.WriteLine($"Created user {email} and logged in");
            }

            else
            {
                ServerSend.SendIntoApp(fromClient, dbID, Server.ConnectedUsersIDList.Count(), false);
                Console.WriteLine("User is already exists"); 
            }




            //int dbID = Server.dataBase.GetUserIDByNickname(message);







            if (fromClient != clientCheck)
            {

                Console.WriteLine($"Player \"\" (ID: {fromClient}) has assumed the wrong client ID ({clientCheck})!");
            }
            // TODO: send user into app
        }
    }
}
