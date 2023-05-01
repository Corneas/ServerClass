using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    // 아직 서버 연동이 안되어있으니, ID로 구분하는 구조는 추후 구현
    List<GameObject> _objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        _objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        _objects.Remove(go);
    }

    public void Clear()
    {
        _objects.Clear();
    }

    // 우선은 심플하게 충돌 구현 - Client Base
    // 모든 게임 오브젝트의 위치 전수 비교
    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject obj in _objects)
        {
            CreatureController creatureController = obj.GetComponent<CreatureController>();
            if (creatureController == null)
                continue;

            if (creatureController.CellPos == cellPos)
                return obj;
        }
        return null;
    }
}
