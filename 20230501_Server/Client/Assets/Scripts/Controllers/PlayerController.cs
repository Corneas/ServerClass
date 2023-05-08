using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
	Coroutine _coSkill;

	protected override void Init()
	{
		base.Init();
	}
	// 상태에 따라 키보드 입력 구분하도록 재구현
	protected override void UpdateController()
	{
		switch (State)
		{
			case CreatureState.Idle:
				GetDirInput();
				GetIdleInput(); // 스킬 관련 메서드
				break;
			case CreatureState.Moving:  // 움직일 땐 스킬 사용불가
				GetDirInput();
				break;
		}
		base.UpdateController();
	}
	void LateUpdate()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
	}

	// 키보드 입력
	void GetDirInput()
	{
		if (Input.GetKey(KeyCode.W))
		{
			Dir = MoveDir.Up;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Dir = MoveDir.Down;
		}
		else if (Input.GetKey(KeyCode.A))
		{
			Dir = MoveDir.Left;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Dir = MoveDir.Right;
		}
		else
		{
			Dir = MoveDir.None;
		}
	}

	// 스킬 애니메이션 관련...
	void GetIdleInput()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			State = CreatureState.Skill;
			_coSkill = StartCoroutine("CoStartPunch");
		}
	}

	// 0.5초마다 스킬 쿨타임 추가
	//IEnumerator CoStartPunch()
	//{
	//	// 피격 판정 - 평타가 나오는 즉시
	//	GameObject go = Managers.Object.Find(GetFrontCellPos());
	//	// GetFrontCellPos : 바로 앞의 Cell의 위치를 반환해주는 간단한 함수
	//	if (go != null)
	//	{
	//		Debug.Log(go.name);
	//	}
	//	// 0.5초 뒤 자동으로 Idle State로 돌아감
	//	yield return new WaitForSeconds(0.5f);
	//	State = CreatureState.Idle;
	//	_coSkill = null;
	//}
}