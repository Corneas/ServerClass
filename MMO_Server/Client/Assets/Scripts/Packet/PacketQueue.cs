using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue Instance { get; } = new PacketQueue();

    Queue<IPacket> _packetQueue = new Queue<IPacket>();
    object _lock = new object();

    // 로그 출력 명령을 저장해둠
    public void Push(IPacket packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(packet);
        }
    }

    // 저장한 로그 출력 명령을 꺼냄
    public IPacket Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count == 0)
                return null;

            return _packetQueue.Dequeue();
        }
    }

    public List<IPacket> PopAll()
    {
        List<IPacket> list = new List<IPacket>();

        lock (_lock)
        {
            while (_packetQueue.Count > 0)
                list.Add(_packetQueue.Dequeue());
        }

        return list;
    }
}
