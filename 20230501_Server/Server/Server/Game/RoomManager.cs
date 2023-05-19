using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();

        // 동시성 제어 위한 락 추가
        object _lock = new object();
        // ID로 룸 구별위해 딕셔너리 구조
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        // 아직 방 여러개를 사용하진 않으니 1로 지정
        int _roomId = 1;
        
        public GameRoom Add()
        {
            GameRoom gameRoom = new GameRoom();

            // 동시에 방 여러개 생기지 않도록 락
            lock (_lock)
            {
                gameRoom.RoomId = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;
        }

        public bool Remove(int roomId)
        {
            // 제거할 때도 락
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public GameRoom Find(int roomId)
        {
            // 찾을 때도 락
            lock (_lock)
            {
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }

    }
}
