using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Arrow : Projectile
	{
		// 나를 쏜게 누군지 체크
		public GameObject Owner { get; set; }

		// 너무 자주 갱신하지 않기 위한 틱 체크 변수
		long _nextMoveTick = 0;

		public override void Update()
		{
			// 쏜 주인도 없고, 방도 없으면 리턴
			if (Owner == null || Room == null)
				return;

			// 아직 시간이 준비되지 않은 경우
			if (_nextMoveTick >= Environment.TickCount64)
				return;

			// 50ms 마다
			_nextMoveTick = Environment.TickCount64 + 50;

			// 화살 이동 연산
			// 내 앞의 위치 매개변수 없이 체크
			Vector2Int destPos = GetFrontCellPos();
			// 갈 수 있는지 체크
			if (Room.Map.CanGo(destPos))
			{
				CellPos = destPos;

				// 패킷에 이동정보 입력
				S_Move movePacket = new S_Move();
				movePacket.ObjectId = Id;
				movePacket.PosInfo = PosInfo;
				// 화살 정보 모두전송
				Room.Broadcast(movePacket);

				Console.WriteLine("Move Arrow");
			}
			else
			{
				GameObject target = Room.Map.Find(destPos);
				if (target != null)
				{
					// TODO : 피격 판정
				}

				// 소멸
				Room.LeaveGame(Id);
			}
		}
	}
}
