using UnityEngine;

public class MainThemeManager : MonoBehaviour
{
    private static MainThemeManager _music;

    private void Awake()
    {
        if (PlayerPrefs.GetString("Music") == "OFF") gameObject.GetComponent<AudioSource>().enabled = false;
        if (_music != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _music = this;
            transform.gameObject.DontDestroyOnLoad();
        }
    }
}