using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    // ���� ���� ������ �ȵǾ�������, ID�� �����ϴ� ������ ���� ����
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

    // �켱�� �����ϰ� �浹 ���� - Client Base
    // ��� ���� ������Ʈ�� ��ġ ���� ��
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
