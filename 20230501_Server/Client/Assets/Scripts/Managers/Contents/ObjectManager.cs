using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	//Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	List<GameObject> _objects = new List<GameObject>();

	public void Add(GameObject go)
	{
		_objects.Add(go);
	}

	public void Remove(GameObject go)
	{
		_objects.Remove(go);
	}

	public GameObject Find(Vector3Int cellPos)
	{
		foreach (GameObject obj in _objects)
		{
			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				continue;

			if (cc.CellPos == cellPos)
				return obj;
		}

		return null;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects)
		{
			// 입력된 목적 오브젝트가 플레이어가 맞다면 플레이어 오브젝트 반환
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
		_objects.Clear();
	}
}
