using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VictoryAnimationsController : MonoBehaviour
{
    [SerializeField] private GameObject victoryBox;
    [SerializeField] private GameObject[] needToHide;

    private void Start()
    {
        PlayVictorySound();
        gameObject.transform.GetComponent<Image>().DOFade(0.6f, 2).OnComplete(() =>
        {
            foreach (var item in needToHide) item.SetActive(false);
            victoryBox.SetActive(true);
            victoryBox.transform.DOScale(1, 1);
        });
    }

    private void PlayVictorySound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF") GetComponent<AudioSource>().Play();
    }
}