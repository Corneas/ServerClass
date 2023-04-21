using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
	

	class Program
	{
		static Listener _listener = new Listener();
		public static GameRoom Room = new GameRoom();

		static void FlushRoom()
        {
			Room.Push(() => Room.Flush());
			JobTimer.Instance.Push(FlushRoom, 250);
        }

		static void Main(string[] args)
		{
			//PacketManager.Instance.Register();

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			int roomTick = 0;
			while (true)
			{
				int now = System.Environment.TickCount;
				// 현재 시간이 저장된 roomTick보다 나중이라면
				if(roomTick < now)
                {
					Room.Push(() => Room.Flush());
					Thread.Sleep(250);
				}
			}
		}
	}
}
