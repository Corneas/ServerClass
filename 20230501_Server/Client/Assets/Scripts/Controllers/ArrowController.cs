using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        // 추후 기능 추가
        switch (_lastDir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        State = CreatureState.Moving;
        _speed = 15.0f;
        base.Init();
    }

    protected override void UpdateAnimation()
    {
        // 추후 기능 추가
    }

    protected override void MoveToNextPos()
    {
        // 움직이지 않을 때 만, 다음 칸 이동하도록
        Vector3Int destPos = CellPos;

        switch (_dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        // 화살이 날라갈 때 물체 있는지 파악
        if (Managers.Map.CanGo(destPos))
        {
            GameObject go = Managers.Object.Find(destPos);
            if(go == null)
            {
                CellPos = destPos;
            }
            else // 피격 판정
            {
                Debug.Log(go.name);
                CreatureController creatureController = go.GetComponent<CreatureController>();
                if(creatureController != null)
                {
                    creatureController.OnDamaged();
                }
                Managers.Resource.Destroy(gameObject);
            }
        }
        else // 화살 자폭
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

}
