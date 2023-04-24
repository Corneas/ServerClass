using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {

            /*using ())*/
            //{
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.31.1.74"), 20000);

                await clientSocket.ConnectAsync(endPoint);

                while(true)
                {
                    string str = Console.ReadLine();
                    //if (str == "exit")
                    //{
                    //    break;
                    //    //return;
                    //}
                    //// 직렬화
                    byte[] buffer = Encoding.UTF8.GetBytes(str);
                    await clientSocket.SendAsync(buffer, SocketFlags.None);

                    //byte[] buffer2 = new byte[256];
                    //int bytesRead = clientSocket.Receive(buffer2);
                    //if(bytesRead < 1)
                    //{
                    //    Console.WriteLine("서버의 연결 종료");
                    //    return;
                    //}

                    //string str2 = Encoding.UTF8.GetString(buffer2);
                    //Console.WriteLine("받음 : " + str2);
                }
            //}
        }
    }
}