using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController music;

    private void Awake()
    {
        if (PlayerPrefs.GetString("Music") == "OFF") gameObject.GetComponent<AudioSource>().enabled = false;
        if (music != null)
        {
            Destroy(gameObject);
        }
        else
        {
            music = this;
            transform.gameObject.DontDestroyOnLoad();
        }
    }
}