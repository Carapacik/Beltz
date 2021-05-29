using UnityEngine;

public class SettingsAnimationController : MonoBehaviour
{
    private GameObject[] _belts;

    private void Start()
    {
        _belts = GameObject.FindGameObjectsWithTag("Belt");
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }

    private void BeltsDisabling()
    {
        foreach (var belt in _belts)
            belt.gameObject.GetComponent<SwapBelts>().enabled = false;
    }

    private void BeltsEnabling()
    {
        foreach (var belt in _belts)
            belt.gameObject.GetComponent<SwapBelts>().enabled = true;
    }
}