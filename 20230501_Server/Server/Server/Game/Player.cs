using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo();    // 플레이어 정보
        public GameRoom Room { get; set; }          // 소속된 룸 정보
        public ClientSession Session { get; set; }  // 어떤 세션에 연결되있는지 정보
    }
}
