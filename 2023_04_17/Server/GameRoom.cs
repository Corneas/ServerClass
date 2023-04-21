using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom
    {
        // 세션을 리스트로 관리
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void BroadCast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = string.Format("{0} I am {1}", chat, packet.playerId); //$"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            // 공유가 시작되는 session은 락으로 관리
            lock (_lock)
            {
                foreach(ClientSession s in _sessions)
                {
                    // 리스트에 들어있는 클라에 모두 전송
                    s.Send(segment);
                }
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session);
            }
        }
    }
}
