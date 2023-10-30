using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateAnimation : MonoBehaviour
{
    public Ease Ease = Ease.Linear;
    public LoopType LoopType = LoopType.Incremental;
    public float Duration = 4f;


    private void Start()
    {
        transform.DORotate(Vector3.up * 360f, Duration)
          .SetEase(Ease)
          .SetLoops(-1, LoopType);
    }
}
