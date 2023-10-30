using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject swipeRightText;
    [SerializeField] private GameObject swipeLeftText;
    [SerializeField] private GameObject swipeDownText;

    private bool swipedRight;
    private bool swipedLeft;

    private void OnEnable()
    {
        if (Managers.Instance == null) return;
        LevelManager.Instance.OnLevelStart.AddListener(EnableTutorial);
        InputManager.Instance.OnSwipeDetected.AddListener(CheckSwipe);
        swipeRightText.SetActive(false);
        swipeLeftText.SetActive(false);
        swipeDownText.SetActive(false);
    }

    private void OnDisable()
    {
        if (Managers.Instance == null) return;
        LevelManager.Instance.OnLevelStart.RemoveListener(EnableTutorial); 
        InputManager.Instance.OnSwipeDetected.RemoveListener(CheckSwipe);
    }

    private void EnableTutorial()
    {
        swipeRightText.SetActive(true);
    }

    private void CheckSwipe(Swipe swipe, Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && direction.x > 0)
        {
            swipedRight = true;
            swipeRightText.transform.DOScale(Vector3.zero, .3f).OnComplete(()=> swipeLeftText.SetActive(true));
        }
        else if (swipedRight && Mathf.Abs(direction.x) > Mathf.Abs(direction.y) && direction.x < 0)
        {
            swipedLeft = true;
            swipeLeftText.transform.DOScale(Vector3.zero, .3f).OnComplete(() => swipeDownText.SetActive(true));
        }
        else if (swipedLeft && Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && direction.y < 0)
        {
            swipeDownText.transform.DOScale(Vector3.zero, .3f);
        }
    }
}
