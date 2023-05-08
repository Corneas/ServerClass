using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coPatrol;
    Coroutine _coSearch;
    Vector3Int _destCellPos;

    GameObject _target; // ���� ��� ����
    float _searchRange = 5.0f; // ��Ī ����

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
        // Ÿ���� �ִٸ�, Ÿ������ ������ ����
        if (_target != null)
            destPos = _target.GetComponent<CreatureController>().CellPos;

        // FindPath = A*�˰��� ȣ��, ignoreDest : ���� �濡 ��ֹ� �־ �浹�� �ν����� �ʴ� ���
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        // ���� ��� �Ÿ� path < 2 : ���� ��ã�� ��� �Ǵ� �ʹ� �־��� ���
        if (path.Count < 2 || (_target != null && path.Count > 10))
        {
            // ��������, Ÿ�� ����
            _target = null;
            State = CreatureState.Idle; // Idle�� ���� ��ȯ
            return;
        }
        // path[0]�� �� ��ġ [1]�� ���� ��ġ, �Ź� ȣ��� ������ path[1]�� �̵�
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
        // �����ϰ� ���
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);
        
        for(int i = 0; i < 10; ++i)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            // ���� �� ��ġ���� -5 ~ +6 ���� �����θ� �̵�
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            // ������ġ�� �̵��� ����Ѱ�?
            if(Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos; // ������� ��ġ destCellPos
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle; // 10�� �õ� ���� ��ã���� Idle�� ��ȯ
    }

    IEnumerator CoSearch()
    {
        // �������� �ٸ��� �׽� ����Ǿ��־�� �ϹǷ� while ����
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1�ʸ��� ��ĵ

            // Ÿ���� �ִ� ��� ����
            if (_target != null)
                continue;

            // ������ Find(Vector3Int) �� Ư����ġ�� ������ �ִ����� üũ,
            // ������ Find(Func<>)�� �ǽð����� Ư����ġ �����Ͽ� üũ
            _target = Managers.Object.Find((go) =>
            {
                // �÷��̾� �ִ���?
                PlayerController pc = go.GetComponent<PlayerController>();

                // �÷��̾� �ƴ� false ����
                if (pc == null)
                    return false;

                // �÷��̾���� �Ÿ� ���
                Vector3Int dir = (pc.CellPos - CellPos);
                // ���� �Ÿ��� Ž���������� �ָ� Ž������
                if(dir.magnitude > _searchRange)
                    return false;

                return true;
            });
        }
    }
}
