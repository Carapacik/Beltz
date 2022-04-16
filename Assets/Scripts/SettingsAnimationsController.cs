using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsAnimationsController : MonoBehaviour
{
    //private GameObject[] _belts;
    [SerializeField] private GameObject settingsBox;
    [SerializeField] private GameObject shadowBackground;

    private void Start()
    {
        //_belts = GameObject.FindGameObjectsWithTag("Belt");
    }

    public void OpenSettings()
    {
        //foreach (var belt in _belts) belt.gameObject.GetComponent<SwapBeltz>().enabled = false;
        shadowBackground.transform.GetComponent<Image>().DOFade(0.5f,1);
        settingsBox.transform.DOLocalMoveY(510, 1);
        ObjectExtension.GetSavedObjects()[0].transform.GetComponent<AudioSource>().DOPitch(0.8f, 1);
    }

    public void CloseSettings()
    {
        //foreach (var belt in _belts) belt.gameObject.GetComponent<SwapBeltz>().enabled = true;
        shadowBackground.transform.GetComponent<Image>().DOFade(0,1);
        settingsBox.transform.DOLocalMoveY(700, 1);
        ObjectExtension.GetSavedObjects()[0].transform.GetComponent<AudioSource>().DOPitch(1, 1)
            .OnComplete(() =>
            {
                shadowBackground.SetActive(false);
                gameObject.SetActive(false);
                
            });
    }
}
