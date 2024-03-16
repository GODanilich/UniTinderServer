using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UniTinderServer
{
    public enum ClientState
    {
      Authorization = 1,
      Registration = 2,
      RegistrationDone = 3,
      Profile = 4,
      Chat = 5,

    }

    class Client
    {

        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public int clientState;


        public Client(int id)
        {
            this.id = id;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            public int idInDatabase;
            private readonly int _id;
            private NetworkStream _stream;
            private Packet _receivedData;
            private byte[] _receiveBuffer;

            public TCP(int id)
            {
                _id = id;
            }

            public void Connect(TcpClient socket)
            {
                this.socket = socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                _receivedData = new Packet(); // инициализация пакета
                _receiveBuffer = new byte[dataBufferSize];

                _stream = socket.GetStream(); // получение потока данных с клиента

                

                Packet packet = new Packet((int)ServerPackets.welcome);
                packet.Write("победа");
                packet.Write(_id);
                packet.WriteLength();
                SendData(packet);
                packet.Dispose();



                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);// асинхронное чтение потока

                //ServerSend.Welcome(_id, $"Welcome to the server! Your ID is {_id}");
            }

            public async void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        //_stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                        await _stream.WriteAsync(packet.ToArray(), 0, packet.Length());
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error sending data to user {_id} via TCP: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult result) // асинхронный коллбэк
            {
                try
                {
                    int byteLength = _stream.EndRead(result); // завершение чтения потока, возвращает его байтовую длину
                    if (byteLength <= 0) 
                    {
                        Server.clients[_id].Disconnect();
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(_receiveBuffer, data, byteLength); // копируем считанные в _receiveBuffer байты в data

                    _receivedData.Reset(HandleData(data)); // ресет пакета для нового использования
                    _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); // асинхронное чтение потока
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error receiving TCP data: {e}");
                    Server.clients[_id].Disconnect();

                }
            }

            private bool HandleData(byte[] data) // 
            {
                int packetLength = 0;

                _receivedData.SetBytes(data); // Заносит принятые данные в буфер пакета и подготавливает для чтения

                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
                while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
                {
                    byte[] packetBytes = _receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            Server.packetHandlers[packetId](_id,packet);
                        }
                    });

                    packetLength = 0;
                    if (_receivedData.UnreadLength() >= 4)
                    {
                        packetLength = _receivedData.ReadInt();
                        if (packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }
                if (packetLength <= 1)
                {
                    return true;
                }

                return false;

            }

            public void Disconnect()
            {
                Server.ConnectedUsersIDList.Remove(idInDatabase);
                socket.Close();
                _stream = null;
                _receivedData = null;
                _receiveBuffer = null;
                socket = null;
            }

        }

        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected");
            tcp.Disconnect();
        }
    }
}
