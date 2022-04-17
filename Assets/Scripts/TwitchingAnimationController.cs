using DG.Tweening;
using UnityEngine;

public class TwitchingAnimationController : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.DOScale(1.3f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}