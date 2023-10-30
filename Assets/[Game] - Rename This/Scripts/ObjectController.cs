using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float cutSpeed = .2f;
    [SerializeField] private ParticleSystem cutExplosionParticle;
    [SerializeField] private float cutTimeLimitDefault = .5f;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private Transform trailTransform;
    [SerializeField] private float minScaleBeforeFail = 0.1f;

    private bool isMoving;
    private bool isBeingCut;
    private bool movingVertically;
    private bool isAnimating;
    private float cutTimer;
    private float cutTimeLimit;
    private BoxCollider col;
    private Vector2 lastSwipeDirection;
    
    private void OnEnable()
    {
        if (Managers.Instance == null) return;
        InputManager.Instance.OnSwipeDetected.AddListener(StartMovement);
        col = GetComponent<BoxCollider>();
        var explosionMain = cutExplosionParticle.main;
        explosionMain.startColor = meshTransform.GetComponent<MeshRenderer>().material.color;
        trailTransform.position = col.bounds.center;
    }

    private void OnDisable()
    {
        if (Managers.Instance == null) return;
        InputManager.Instance.OnSwipeDetected.RemoveListener(StartMovement);
    }

    private void StartMovement(Swipe swipe, Vector2 direction)
    {
        if (isBeingCut || isAnimating || isMoving) return;

        lastSwipeDirection = direction;
        Vector3 movementDir = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? (Vector3.right * (direction.x > 0 ? 1 : -1)) : (Vector3.forward * (direction.y > 0 ? 1 : -1));
        RaycastHit hit;
        Vector3 movementVector = Vector3.zero;
        movingVertically = movementDir.x == 0;

        if (Physics.BoxCast(col.bounds.center, col.bounds.extents * 0.99f, movementDir, out hit))
        {
            if (movingVertically) movementVector = new Vector3(0, 0, hit.point.z - col.bounds.center.z + (movementDir.z < 0 ? col.bounds.extents.z : -col.bounds.extents.z));
            else movementVector = new Vector3(hit.point.x - col.bounds.center.x + (movementDir.x < 0 ? col.bounds.extents.x : -col.bounds.extents.x), 0, 0);
        }
        else movementVector = movementDir * 50;

        isBeingCut = false;
        isMoving = true;
        cutTimer = 0;
        cutExplosionParticle.Stop();
        transform.DOMove(transform.position + movementVector, movementVector.magnitude / moveSpeed).SetEase(Ease.InOutSine).OnUpdate(()=> HapticManager.Haptic(HapticTypes.SoftImpact)).OnComplete(() =>
        {
        isMoving = false;
        if (hit.transform.parent.TryGetComponent(out Sandpaper sandpaper))
        {
            isBeingCut = true;
            cutTimeLimit = cutTimeLimitDefault * sandpaper.CutDurationMultiplier;
                }
        });
        if (movingVertically)
        {
            var scaleZ = meshTransform.localScale.z;
            isAnimating = true;
            meshTransform.DOScaleZ(scaleZ / 1.5f, .1f).OnComplete(() => meshTransform.DOScaleZ(scaleZ, .1f).OnComplete(() => isAnimating = false));
        }
        else
        {
            var scaleX = meshTransform.localScale.x;
            isAnimating = true;
            meshTransform.DOScaleX(scaleX / 1.5f, .1f).OnComplete(() => meshTransform.DOScaleX(scaleX, .1f).OnComplete(() => isAnimating = false));
        }
    }

    private void CutObject()
    {
        if (movingVertically)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - cutSpeed * Time.deltaTime);
            transform.position += (Vector3.forward * (lastSwipeDirection.y > 0 ? 1 : -1)) * cutSpeed * Time.deltaTime;
            cutExplosionParticle.transform.position = col.bounds.center + new Vector3(0, 0, lastSwipeDirection.y) * col.bounds.extents.z;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x - cutSpeed * Time.deltaTime, transform.localScale.y, transform.localScale.z);
            transform.position += (Vector3.right * (lastSwipeDirection.x > 0 ? 1 : -1)) * cutSpeed * Time.deltaTime;
            cutExplosionParticle.transform.position = col.bounds.center + new Vector3(lastSwipeDirection.x, 0, 0) * col.bounds.extents.x;
        }
        if (transform.localScale.x < minScaleBeforeFail || transform.localScale.z < minScaleBeforeFail)
        {
            GameManager.Instance.CompeleteStage(false);
            isBeingCut = false;
            cutExplosionParticle.Stop();
        }
        if (!cutExplosionParticle.isPlaying) cutExplosionParticle.Play();
    }

    private void Update()
    {
        if (isBeingCut)
        {
            HapticManager.Haptic(HapticTypes.HeavyImpact);
            cutTimer += Time.deltaTime;
            if (cutTimer > cutTimeLimit)
            {
                cutExplosionParticle.Stop();
                isBeingCut = false;
                cutTimer = 0;
            }
            CutObject();
        }
    }
}
