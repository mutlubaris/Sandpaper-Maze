using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum MoveDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }


public class MoveAnimation : MonoBehaviour
{
    public MoveDirection MoveDirection = MoveDirection.Right;
    public Ease Ease = Ease.Linear;
    public float Duration = 2f;
    public float MoveDistance = 100f;

    private static readonly Vector3 upRight = new Vector3(1, 1, 0);
    private static readonly Vector3 upLeft = new Vector3(-1, 1, 0);
    private static readonly Vector3 downRight = new Vector3(1, -1, 0);
    private static readonly Vector3 downLeft = new Vector3(-1, -1, 0);

    private void Start()
    {
        switch (MoveDirection)
        {       
            case MoveDirection.Up:
                transform.DOLocalMoveY(transform.localPosition.y + MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.Down:
                transform.DOLocalMoveY(transform.localPosition.y - MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.Left:
                transform.DOLocalMoveX(transform.localPosition.x - MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.Right:   
                transform.DOLocalMoveX(transform.localPosition.x + MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.UpLeft:
                transform.DOLocalMove(transform.localPosition + upRight * MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.UpRight:
                transform.DOLocalMove(transform.localPosition + upLeft * MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.DownLeft:
                transform.DOLocalMove(transform.localPosition + downLeft * MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            case MoveDirection.DownRight:
                transform.DOLocalMove(transform.localPosition + downRight * MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
            default://Move Right
                transform.DOLocalMoveX(transform.localPosition.x + MoveDistance, Duration).SetEase(Ease).SetLoops(-1, LoopType.Restart);
                break;
        }
    }
}
