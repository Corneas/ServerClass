using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{
	

	class Program
	{
		static void Main(string[] args)
		{
			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			Connector connector = new Connector();

			// 10개의 클라이언트 접속
			connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, 100);

			while (true)
			{
				try
				{
					SessionManager.Instance.SendForEach();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}

				// 1초에 4번 전송
				Thread.Sleep(250);
			}
		}
	}
}
