using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();

        // 동시성 제어 위한 락 추가
        object _lock = new object();
        // ID로 룸 구별위해 딕셔너리 구조
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        // 아직 방 여러개를 사용하진 않으니 1로 지정
        int _playerId = 1;

        public Player Add()
        {
            Player player = new Player();

            // 동시에 방 여러개 생기지 않도록 락
            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;
            }

            return player;
        }

        public bool Remove(int playerId)
        {
            // 제거할 때도 락
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            // 찾을 때도 락
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                    return player;

                return null;
            }
        }

    }
}
