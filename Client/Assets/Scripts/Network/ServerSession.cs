using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using ServerCore;

namespace DummyClient
{
	class ServerSession : PacketSession
	{
		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

//			C_PlayerInfoReq packet = new C_PlayerInfoReq() { playerId = 1001, name = "ABCD" };
			

			// 보낸다
			//for (int i = 0; i < 5; i++)
			//{
			//	ArraySegment<byte> s = packet.Write();
			//	if (s != null)
			//		Send(s);
			//}
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);

			//string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
			//Console.WriteLine($"[From Server] {recvData}");
			//return buffer.Count;
		}

		public override void OnSend(int numOfBytes)
		{

			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
