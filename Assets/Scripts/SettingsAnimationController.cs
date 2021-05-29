using UnityEngine;

public class SettingsAnimationController : MonoBehaviour
{
    private GameObject[] _belts;
    private AudioSource _music;


    private void Start()
    {
        _music = ObjectExtension.GetSavedObjects()[0].transform.GetComponent<AudioSource>();
        _belts = GameObject.FindGameObjectsWithTag("Belt");
    }

    private void OnClose()
    {
        VictoryController.SwapAllBelts(_belts, true);
        _music.pitch = 1f;
        gameObject.SetActive(false);
    }

    private void OnOpen()
    {
        VictoryController.SwapAllBelts(_belts, false);
        _music.pitch = 0.85f;
    }
}