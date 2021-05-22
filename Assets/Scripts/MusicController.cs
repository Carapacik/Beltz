using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController _music;

    private void Awake()
    {
        if (_music != null || PlayerPrefs.GetString("Music") == "OFF")
        {
            if (PlayerPrefs.GetString("Music") == "OFF")
                gameObject.GetComponent<AudioSource>().enabled = false;
            else
                Destroy(gameObject);
        }
        else
        {
            _music = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}