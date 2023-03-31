using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS
            IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = iphost.AddressList[1];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 7777);

            while (true)
            {

                // 휴대폰 설정
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 문지기에게 입장 문의
                    // 상대방 주소 입력
                    socket.Connect(endPoint);
                    // 연결시, 서버 정보 출력
                    Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                    // 보냄
                    byte[] sentBuff = Encoding.UTF8.GetBytes("Hello World!");
                    int sendBytes = socket.Send(sentBuff);

                    // 받음
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[FROM SERVER] {recvData}");

                    // 나감
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
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
