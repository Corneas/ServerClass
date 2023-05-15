using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coSkill; // 몬스터 스킬
    Coroutine _coPatrol;
    Coroutine _coSearch;
    Vector3Int _destCellPos;

    GameObject _target; // 공격 대상 변수
    float _searchRange = 5.0f; // 서칭 범위
    
    [SerializeField]
    private float _skillRange = 1.0f; // 스킬공격범위 1

    private bool _rangeSkill = false;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;

            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }

            if (_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        State = CreatureState.Idle;
        Dir = MoveDir.None;

        _speed = 3.0f;
        _rangeSkill = (Random.Range(0, 2) == 0 ? true : false);

        if (_rangeSkill)
            _skillRange = 10.0f;
        else
            _skillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        if (_coPatrol == null)
        {
            _coPatrol = StartCoroutine("CoPatrol");
        }
        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        // 타겟이 있다면, 타겟으로 목적지 설정
        if (_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            // 타겟위치 - 몬스터위치
            Vector3Int dir = destPos - CellPos;
            // 타겟이 스킬범위 내 일때
            if (dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                // 스킬 사용 전, 방향 지정
                Dir = GetDirFromVec(dir);
                // 스킬 사용 상태로 전환
                State = CreatureState.Skill;

                if (_rangeSkill)
                    _coSkill = StartCoroutine(CoStartShootArrow()); // 화살공격
                else
                    _coSkill = StartCoroutine(CoStartPunch());  // 펀치

                return;
            }
        }

        // FindPath = A*알고리즘 호출, ignoreDest : 가는 길에 장애물 있어도 충돌로 인식하지 않는 기능
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        // 가는 경로 거리 path < 2 : 길을 못찾은 경우 또는 너무 멀어진 경우
        if (path.Count < 2 || (_target != null && path.Count > 10))
        {
            // 추적포기, 타겟 삭제
            _target = null;
            State = CreatureState.Idle; // Idle로 상태 전환
            return;
        }
        // path[0]은 내 위치 [1]이 다음 위치, 매번 호출될 때마다 path[1]로 이동
        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        if (moveCellDir.x > 0)
            Dir = MoveDir.Right;
        else if (moveCellDir.x < 0)
            Dir = MoveDir.Left;
        else if (moveCellDir.y > 0)
            Dir = MoveDir.Up;
        else if (moveCellDir.y < 0)
            Dir = MoveDir.Down;
        else
            Dir = MoveDir.None;

        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }

    IEnumerator CoPatrol()
    {
        // 랜덤하게 대기
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);
        
        for(int i = 0; i < 10; ++i)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            // 현재 내 위치에서 -5 ~ +6 범위 정도로만 이동
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            // 랜덤위치로 이동이 사능한가?
            if(Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos; // 가고싶은 위치 destCellPos
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle; // 10번 시도 끝에 못찾으면 Idle로 전환
    }

    IEnumerator CoSearch()
    {
        // 정찰과는 다르게 항시 실행되어있어야 하므로 while 루프
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1초마다 스캔

            // 타겟이 있는 경우 진행
            if (_target != null)
                continue;

            // 기존의 Find(Vector3Int) 는 특정위치에 무엇이 있는지만 체크,
            // 개선된 Find(Func<>)는 실시간으로 특정위치 전달하여 체크
            _target = Managers.Object.Find((go) =>
            {
                // 플레이어 있는지?
                PlayerController pc = go.GetComponent<PlayerController>();

                // 플레이어 아님 false 반한
                if (pc == null)
                    return false;

                // 플레이어와의 거리 계산
                Vector3Int dir = (pc.CellPos - CellPos);
                // 만약 거리가 탐색범위보다 멀면 탐색종료
                if(dir.magnitude > _searchRange)
                    return false;

                return true;
            });
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
        //_rangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    // 화살 스킬
    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController arrowController = go.GetComponent<ArrowController>();
        arrowController.Dir = _lastDir;     // 플레이어와 화살의 방향 같아야
        arrowController.CellPos = CellPos;  // 내 위치에서 발사

        // 0.3초 마다 발사
        //_rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
