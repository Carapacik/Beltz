using UnityEngine;

public class SettingsAnimationController : MonoBehaviour
{
    private GameObject[] _belts;

    private void Start()
    {
        _belts = GameObject.FindGameObjectsWithTag("Belt");
    }

    private void OnOpen()
    {
        ObjectExtension.GetSavedObjects()[0].transform.GetComponent<AudioSource>().pitch = .85f;
        foreach (var belt in _belts) belt.gameObject.GetComponent<SwapBelts>().enabled = false;
    }

    private void OnClose()
    {
        ObjectExtension.GetSavedObjects()[0].transform.GetComponent<AudioSource>().pitch = 1;
        foreach (var belt in _belts) belt.gameObject.GetComponent<SwapBelts>().enabled = true;
    }

    private void HideSettingsObject()
    {
        gameObject.SetActive(false);
    }
}