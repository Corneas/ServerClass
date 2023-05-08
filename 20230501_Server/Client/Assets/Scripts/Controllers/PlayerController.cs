using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
	Coroutine _coSkill;
	bool _rangeSkill = false; // 추후 단일, 범위스킬 구분

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

    protected override void UpdateAnimation()
    {
        if (_state == CreatureState.Idle)
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
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
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
        // 크리쳐 스킬 애니메이션 파트
        else if (_state == CreatureState.Skill)
        {
            // TODO
            switch (_lastDir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {
            // 죽는 애니메이션?
        }
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

    protected override void UpdateIdle()
    {
        if(Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            //_coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    // 0.5초마다 스킬 쿨타임 추가
    IEnumerator CoStartPunch()
    {
        // 피격 판정 - 평타가 나오는 즉시
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        // GetFrontCellPos : 바로 앞의 Cell의 위치를 반환해주는 간단한 함수
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
            {
                cc.OnDamaged();
            }
            Debug.Log(go.name);
        }
        // 0.5초 뒤 자동으로 Idle State로 돌아감
        _rangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    // 화살 스킬

    IEnumerator CoStartShootArrow()
    {
		GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
		ArrowController arrowController = go.GetComponent<ArrowController>();
		arrowController.Dir = _lastDir;		// 플레이어와 화살의 방향 같아야
		arrowController.CellPos = CellPos;  // 내 위치에서 발사

        // 0.3초 마다 발사
        _rangeSkill = true;
		yield return new WaitForSeconds(0.3f);
		State = CreatureState.Idle;
		_coSkill = null;
    }
}