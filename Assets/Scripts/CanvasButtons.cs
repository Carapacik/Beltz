using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButtons : MonoBehaviour
{
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;

    private void Start()
    {
        if (PlayerPrefs.GetString("Sound") == "OFF" && gameObject.name == "SoundButton")
            GetComponent<Image>().sprite = soundOff;
        if (PlayerPrefs.GetString("Music") == "OFF" && gameObject.name == "MusicButton")
            GetComponent<Image>().sprite = musicOff;
    }

    public void NextScene()
    {
        PlayClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ChooseLevel(int number)
    {
        PlayClickSound();
        SceneManager.LoadScene($"Lvl{number}");
    }

    public void LevelMenu()
    {
        PlayClickSound();
        SceneManager.LoadScene("LevelMenu");
    }

    public void SwitchSound()
    {
        if (PlayerPrefs.GetString("Sound") == "ON")
        {
            PlayerPrefs.SetString("Sound", "OFF");
            GetComponent<Image>().sprite = soundOff;
        }
        else
        {
            PlayerPrefs.SetString("Sound", "ON");
            GetComponent<Image>().sprite = soundOn;
        }
    }

    public void SwitchMusic()
    {
        if (PlayerPrefs.GetString("Music") == "ON")
        {
            PlayerPrefs.SetString("Music", "OFF");
            GetComponent<Image>().sprite = musicOff;
        }
        else
        {
            PlayerPrefs.SetString("Music", "ON");
            GetComponent<Image>().sprite = musicOn;
        }
    }

    public void PlayClickSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF")
            GetComponent<AudioSource>().Play();
    }
}