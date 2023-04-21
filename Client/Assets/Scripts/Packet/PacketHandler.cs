using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using DummyClient;

class PacketHandler
{
	public static void S_ChatHandler(PacketSession session, IPacket packet)
	{
		//
		S_Chat chatPacket = packet as S_Chat;
		ServerSession serverSession = session as ServerSession;

		// 특정 경우, 1번 세션만 메시지 출력
		if(chatPacket.playerId == 1)
            Console.WriteLine(chatPacket.chat);
	}
}
