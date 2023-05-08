﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DummyClient
{
	class SessionManager
	{
		static SessionManager _session = new SessionManager();
		public static SessionManager Instance { get { return _session; } }

		List<ServerSession> _sessions = new List<ServerSession>();
		object _lock = new object();
		Random _rand = new Random();

		//public void SendForEach()
		//{
		//	lock (_lock)
		//	{
		//		foreach (ServerSession session in _sessions)
		//		{
		//			C_Chat chatPacket = new C_Chat();
		//			chatPacket.chat = $"Hello Server !";
		//			ArraySegment<byte> segment = chatPacket.Write();

		//			session.Send(segment);
		//		}
		//	}
		//}

		public void SendForEach()
		{
			lock (_lock)
			{
				foreach (ServerSession session in _sessions)
				{
					C_Move movePacket = new C_Move();
					movePacket.posX = _rand.Next(-50, 50);
					movePacket.posY = 1;
					movePacket.posZ = _rand.Next(-50, 50);

					// 서버쪽에 전송
					session.Send(movePacket.Write());
				}
			}
		}

		public ServerSession Generate()
		{
			lock (_lock)
			{
				ServerSession session = new ServerSession();
				_sessions.Add(session);
				return session;
			}
		}
	}
}
