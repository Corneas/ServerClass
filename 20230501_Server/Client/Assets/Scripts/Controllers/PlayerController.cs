﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Rendering;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
	protected Coroutine _coSkill;
	bool _rangedSkill = false;

	protected override void Init()
	{
		base.Init();
	}

	protected override void UpdateAnimation()
	{
		if (PosInfo.State == CreatureState.Idle)
		{
			switch (_lastDir)
			{
				case MoveDir.Up:
					_animator.Play("IDLE_BACK");
					_sprite.flipX = false;
					break;
				case MoveDir.Down:
					_animator.Play("IDLE_FRONT");
					_sprite.flipX = false;
					break;
				case MoveDir.Left:
					_animator.Play("IDLE_RIGHT");
					_sprite.flipX = true;
					break;
				case MoveDir.Right:
					_animator.Play("IDLE_RIGHT");
					_sprite.flipX = false;
					break;
			}
		}
		else if (State == CreatureState.Moving)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play("WALK_BACK");
					_sprite.flipX = false;
					break;
				case MoveDir.Down:
					_animator.Play("WALK_FRONT");
					_sprite.flipX = false;
					break;
				case MoveDir.Left:
					_animator.Play("WALK_RIGHT");
					_sprite.flipX = true;
					break;
				case MoveDir.Right:
					_animator.Play("WALK_RIGHT");
					_sprite.flipX = false;
					break;
			}
		}
		else if (PosInfo.State == CreatureState.Skill)
		{
			switch (_lastDir)
			{
				case MoveDir.Up:
					_animator.Play(_rangedSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
					_sprite.flipX = false;
					break;
				case MoveDir.Down:
					_animator.Play(_rangedSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
					_sprite.flipX = false;
					break;
				case MoveDir.Left:
					_animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
					_sprite.flipX = true;
					break;
				case MoveDir.Right:
					_animator.Play(_rangedSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
					_sprite.flipX = false;
					break;
			}
		}
		else
		{

		}
	}

	protected override void UpdateController()
	{
		//switch (State)
		//{
		//	case CreatureState.Idle:
		//		GetDirInput();
		//		break;
		//	case CreatureState.Moving:
		//		GetDirInput();
		//		break;
		//}
		
		base.UpdateController();
	}

	protected override void UpdateIdle()
	{
		// 이동 상태로 갈지 확인
		if (Dir != MoveDir.None)
		{
			State = CreatureState.Moving;
			return;
		}

		//// 스킬 상태로 갈지 확인
		//if (Input.GetKey(KeyCode.Space))
		//{
		//	State = CreatureState.Skill;
		//	//_coSkill = StartCoroutine("CoStartPunch");
		//	_coSkill = StartCoroutine("CoStartShootArrow");
		//}
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

	IEnumerator CoStartPunch()
	{
		//// 피격 판정
		//GameObject go = Managers.Object.Find(GetFrontCellPos());
		//if (go != null)
		//{
		//	CreatureController cc = go.GetComponent<CreatureController>();
		//	if (cc != null)
		//		cc.OnDamaged();
		//}

		// 대기 시간
		_rangedSkill = false;
		State = CreatureState.Skill;
		// 스킬 사용 여부는 서버에서 체크해주기도 하지만, 클라쪽에서도 대기를 걸어 무한정한 스킬 사용 요청을 방지
		yield return new WaitForSeconds(0.5f);

		State = CreatureState.Idle;
		_coSkill = null;
		CheckUpdatedFlag();
	}

	IEnumerator CoStartShootArrow()
	{
		GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
		ArrowController ac = go.GetComponent<ArrowController>();
		ac.Dir = _lastDir;
		ac.CellPos = CellPos;

		// 대기 시간
		_rangedSkill = true;
		yield return new WaitForSeconds(0.3f);
		State = CreatureState.Idle;
		_coSkill = null;
	}

	public override void OnDamaged()
	{
		Debug.Log("Player HIT !");
	}

	public void UseSkill(int skillId)
    {
		if(skillId == 1)
        {
			_coSkill = StartCoroutine(CoStartPunch());
        }
    }

	protected virtual void CheckUpdatedFlag()
    {

    }
}
