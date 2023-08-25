using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace UDPServer
{
    class Program
    {
        const int SERVERPORT = 9000;
        const int BUFSIZE = 512;

        static void Main(string[] args)
        {
            int retval;

            Socket socket = null;

            try
            {
                // 소켓 생성
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // 포트 설정
                socket.Bind(new IPEndPoint(IPAddress.Any, SERVERPORT));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            byte[] buf = new byte[BUFSIZE];

            while (true)
            {
                try
                {
                    IPEndPoint anyaddress = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint peeraddress = (EndPoint)anyaddress;
                    retval = socket.ReceiveFrom(buf, BUFSIZE, SocketFlags.None, ref peeraddress);

                    Console.WriteLine("[UDP/{0}:{1}] {2}", 
                        ((IPEndPoint)peeraddress).Address, 
                        ((IPEndPoint)peeraddress).Port,
                        Encoding.Default.GetString(buf,0,retval));

                    // 데이터 송신
                    // socket.SendTo(buf, 0, retval, SocketFlags.None, peeraddress);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            socket.Close();
        }
    }
}
