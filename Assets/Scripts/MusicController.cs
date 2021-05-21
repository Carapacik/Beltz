using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController _instanse;

    private void Awake()
    {
        if (_instanse != null || PlayerPrefs.GetString("Music") == "OFF")
        {
            if (PlayerPrefs.GetString("Music") == "OFF")
            {
                gameObject.GetComponent<AudioSource>().enabled = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            _instanse = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }
}