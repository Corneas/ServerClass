using System;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class LoginOkPacket : Packet
    {

    }

    class GameSession : PacketSession
    {
        class Knight
        {
            public int hp;
            public int attack;
        }

        public override void OnConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"OnConnected : {endPoint}");

            //Knight knight = new Knight() { hp = 100, attack = 100 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer = BitConverter.GetBytes(knight.hp);
            //byte[] buffer2 = BitConverter.GetBytes(knight.attack);

            //// Array.Copy(원본 배열, 원본배열 복사 위치, 복사될 배열, 복사될 배열 시작 위치, 길이)
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            //Send(sendBuff);

            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected bytes : {endPoint}");
        }

        //public override int OnRecv(ArraySegment<byte> buffer)
        //{
        //    string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        //    Console.WriteLine($"[ FROM CLIENT ] {recvData}");
        //    return buffer.Count;
        //}

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId : {id}, Size {size}");
        }
    }

    internal class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // DNS
            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iphost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // IP주소, 포트번호 입력

            _listener.Init(endPoint, () => { return new GameSession(); });  // 1번
            Console.WriteLine("Listening...(영업중이야)");

            while (true)
            {
                //프로그램 종료 막기 위해 while
            }
        }

        //static void OnAcceptHandler(Socket clientSocket) // 7번
        //{
        //    try
        //    {
        //        #region 초기
        //        //// 받는다.
        //        //byte[] recvBuff = new byte[1024];
        //        //int recvBytes = clientSocket.Receive(recvBuff); // 1. 데이터를 recvBuff에 받고, 2. 받은 데이터량을 계산한다.
        //        //string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
        //        //Console.WriteLine($"[FROM CLIENT] {recvData}");

        //        //// 보낸다. (받기의 역순)
        //        //byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To the Jungle!");
        //        //clientSocket.Send(sendBuff);

        //        //// 빠이빠이, 쫓아내기
        //        //clientSocket.Shutdown(SocketShutdown.Both); // 종료 예고
        //        //clientSocket.Close();
        //        #endregion

        //        #region Session 사용
        //        Session session = new GameSession();
        //        session.Init(clientSocket); 

        //        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome To the Jungle!");
        //        session.Send(sendBuff);

        //        Thread.Sleep(1000);

        //        session.Disconnect();
        //        session.Disconnect();
        //        #endregion
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}
    }
}
