using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using Server;

class PacketHandler
{
	public static void C_ChatHandler(PacketSession session, IPacket packet)
	{
		C_Chat chatPacket = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;

		// 방에 없는 상태
		if(clientSession.Room == null)
        {
			return;
        }

		//clientSession.Room.BroadCast(clientSession, chatPacket.chat);
		clientSession.Room.Push(() => clientSession.Room.BroadCast(clientSession, chatPacket.chat));
	}
}
