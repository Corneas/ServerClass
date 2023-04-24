using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using(Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.31.1.74"), 20000);

                serverSocket.Bind(endPoint);
                
                serverSocket.Listen(20);

                while (true)
                {
                    //using (Socket clientSocket = serverSocket.Accept())
                    //{
                        Socket clientSocket = await serverSocket.AcceptAsync();
                        Console.WriteLine(clientSocket.RemoteEndPoint);
                        ThreadPool.QueueUserWorkItem(ReadAsync, clientSocket);

                        //Thread t1 = new Thread(() =>
                        //{
                        //    while (true)
                        //    {
                        //        byte[] buffer = new byte[256];
                        //        int totalByte = clientSocket.Receive(buffer);

                        //        if (totalByte < 1)
                        //        {
                        //            Console.WriteLine("클라이언트의 연결종료");
                        //            clientSocket.Dispose();
                        //            return;
                        //        }
                        //        // 역직렬화
                        //        string str = Encoding.UTF8.GetString(buffer);
                        //        Console.WriteLine(str);

                        //        clientSocket.Send(buffer);
                        //    }
                        //});
                        //t1.Start();
                    //}
                }
            }

            //Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("172.31.1.74"), 20000);

            //serverSocket.Bind(endPoint);

            //// 연결 요청 대기방 (20개)
            //serverSocket.Listen(20);

            //// 연결 요청 수락
            //Socket clientSocket = serverSocket.Accept();

            //Console.WriteLine("연결됨 : " + clientSocket.RemoteEndPoint);
        }

        private static async void ReadAsync(object? sender)
        {
            Socket clientSocket = (Socket)sender;
            while (true)
            {
                byte[] buffer = new byte[256];
                int n1 = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                if(n1 < 1)
                {
                    Console.WriteLine("client disconnect");
                    clientSocket.Dispose();
                    return;
                }
                Console.WriteLine(Encoding.UTF8.GetString(buffer));
            }
        }
    }

}