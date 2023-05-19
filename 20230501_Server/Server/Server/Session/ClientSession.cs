using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;

namespace Server
{
	class ClientSession : PacketSession
	{
		public Player MyPlayer { get; set; }	// 플레이어 구분
		public int SessionId { get; set; }

		public void Send(IMessage packet)
        {
			// 패킷에서 S_Chat 인지 가져옴
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
			// msgName과 일치하는 enum값 가져옴
			MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
			
			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
			// ushort protocolId = (ushort)MsgId.SChat;
			Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			// PROTO Test

			// 플레이어 정보 추가 위한 패킷 생성
			MyPlayer = PlayerManager.Instance.Add();
            {
				// 플레이어 아이디를 네임으로
				MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}";
				MyPlayer.Info.PosInfo.State = CreatureState.Idle;
				MyPlayer.Info.PosInfo.MoveDir = MoveDir.None;
				MyPlayer.Info.PosInfo.PosX = 0;
				MyPlayer.Info.PosInfo.PosY = 0;
				// 현재 세션
				MyPlayer.Session = this;
            }

			RoomManager.Instance.Find(1).EnterGame(MyPlayer);

			//S_Chat chat = new S_Chat()
			//{
			//	Context = "안녕하세요"
			//};

			//S_Chat chat2 = new S_Chat();
			//chat2.MergeFrom(sendBuffer, 4, sendBuffer.Length - 4);
			//////////////////////////
			//////////////////////////
			//Program.Room.Push(() => Program.Room.Enter(this));

			//Send(chat);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
