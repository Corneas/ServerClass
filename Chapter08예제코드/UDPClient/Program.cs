using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UDPClient
{
    class Program
    {
        static string SERVERIP = "255.255.255.255";
        const int SERVERPORT = 9000;
        const int BUFSIZE = 512;

        static void Main(string[] args)
        {
            int retval;

            Socket socket = null;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(SERVERIP), SERVERPORT);
            byte[] buf = new byte[BUFSIZE];

            while(true)
            {
                Console.Write("\n[보낼 데이터] ");
                string data = Console.ReadLine();
                if (data.Length == 0) break;

                try
                {
                    // 데이터 send
                    byte[] sendData = Encoding.Default.GetBytes(data);
                    int size = sendData.Length;
                    if (size > BUFSIZE) size = BUFSIZE;
                    retval = socket.SendTo(sendData, 0, size, SocketFlags.None, serverAddress);
                    Console.WriteLine("[UDP 클라이언트] {0} 바이트 전송", retval);

                    //IPEndPoint anyAddress = new IPEndPoint(IPAddress.Any, 0);
                    //EndPoint peerAddress = (EndPoint)anyAddress;
                    //retval = socket.ReceiveFrom(buf, BUFSIZE, SocketFlags.None, ref peerAddress);

                    //Console.WriteLine("[UDP 클라이언트] {0} 바이트 수신", retval);
                    //Console.WriteLine("[받은 데이터] {0}", Encoding.Default.GetString(buf, 0, retval));

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            socket.Close();

        }
    }
}
