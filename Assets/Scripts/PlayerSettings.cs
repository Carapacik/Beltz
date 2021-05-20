using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetString("Sound", "ON");
        PlayerPrefs.SetString("Music", "ON");
    }

    private void Update()
    {
    }
}