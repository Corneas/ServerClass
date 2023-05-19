using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyPlayerController MyPlayer { get; set; }
	// ID에 따라 Object 구분 (MyPlayer, Player, Monster, Item, etc)
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	//List<GameObject> _objects = new List<GameObject>();

	public void Add(PlayerInfo info, bool myPlayer = false)
	{
        // 내 플레이어일 때
        if (myPlayer)
        {
			GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
			go.name = info.Name;
			_objects.Add(info.PlayerId, go);

			MyPlayer = go.GetComponent<MyPlayerController>();
			MyPlayer.Id = info.PlayerId;
			MyPlayer.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
        }
		else
		{
			GameObject go = Managers.Resource.Instantiate("Creature/Player");
			go.name = info.Name;
			_objects.Add(info.PlayerId, go);

			PlayerController pc = go.GetComponent<PlayerController>();
			pc.Id = info.PlayerId;
			pc.CellPos = new Vector3Int(info.PosX, info.PosY, 0);
		}
	}

	public void Remove(int id)
	{
		_objects.Remove(id);
	}

	public void RemoveMyPlayer()
    {
		if (MyPlayer == null)
			return;

		Remove(MyPlayer.Id);
		MyPlayer = null;
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
