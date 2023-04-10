using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected bytes : {endPoint}");

            Packet packet = new Packet() { size = 4, packetId = 7 };
            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine($"OnConnected : {endPoint}");

                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                byte[] buffer = BitConverter.GetBytes(packet.size);
                byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

                // Array.Copy(원본 배열, 원본배열 복사 위치, 복사될 배열, 복사될 배열 시작 위치, 길이)
                Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

                Send(sendBuff);
            }
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected bytes : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[ FROM Server ] {recvData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // DNS
            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = iphost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return new GameSession(); });

            while (true)
            {

                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(100);
            }

        }
    }
}
