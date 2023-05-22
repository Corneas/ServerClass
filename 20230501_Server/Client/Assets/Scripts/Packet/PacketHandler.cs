using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_EnterGameHandler");
		//Debug.Log(enterGamePacket.Player);
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
		Managers.Object.RemoveMyPlayer();
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_LeaveGameHandler");
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;

		foreach(PlayerInfo player in spawnPacket.Players)
        {
			Managers.Object.Add(player, myPlayer: false);
        }
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_SpawnHandler");
		//Debug.Log(spawnPacket.Players);
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach(int id in despawnPacket.PlayerIds)
        {
			Managers.Object.Remove(id);
        }
		//ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_DespawnHandler");
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		//Debug.Log("S_MoveHandler");

		// 누가 이동하는지 찾기
		GameObject go = Managers.Object.FindById(movePacket.PlayerId);
		// 못찾으면 종료
		if(go == null)
			return;

		// 크리쳐 정보 수정을 위해, 크리쳐 컨트롤러 접근
		CreatureController cc = go.GetComponent<CreatureController>();
		if(cc == null)
			return;

		// S_Move의 값을 바로 크리쳐의 PosInfo로 넘겨준다. (데이터 수신)
		cc.PosInfo = movePacket.PosInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.PlayerId);
		// 못찾으면 종료
		if (go == null)
			return;

		PlayerController pc = go.GetComponent<PlayerController>();
		if(pc != null)
        {
			pc.UseSkill(skillPacket.Info.SkillId);
        }
	}
}
