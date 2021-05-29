using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController music;

    private void Awake()
    {
        if (music != null || PlayerPrefs.GetString("Music") == "OFF")
        {
            if (PlayerPrefs.GetString("Music") == "OFF")
                gameObject.GetComponent<AudioSource>().enabled = false;
            else
                Destroy(gameObject);
        }
        else
        {
            music = this;
            transform.gameObject.DontDestroyOnLoad();
        }
    }
}