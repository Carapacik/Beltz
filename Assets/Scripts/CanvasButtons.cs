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

    public void Play()
    {
        var lastLvl = PlayerPrefs.GetInt("LastCompletedLevel");
        if (lastLvl != 0)
        {
            ChooseLevel(lastLvl);
        }
        else
        {
            PlayClickSound();
            LoadLvl1();
        }
    }

    private static void LoadLvl1()
    {
        SceneManager.LoadScene("Lvl1");
    }

    public void ChooseLevel(int number)
    {
        var lastLvl = PlayerPrefs.GetInt("LastCompletedLevel");
        if (number <= lastLvl)
        {
            PlayClickSound();
            SceneManager.LoadScene($"Lvl{number}");
        }
        else if (lastLvl == 0)
        {
            PlayClickSound();
            LoadLvl1();
        }
        else
        {
            PlayUnavailableSound();
        }
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
            PlayClickSound();
        }
    }

    public void SwitchMusic()
    {
        if (PlayerPrefs.GetString("Music") == "ON")
        {
            PlayerPrefs.SetString("Music", "OFF");
            GetComponent<Image>().sprite = musicOff;
            GameObject.Find("MainTheme").GetComponent<AudioSource>().enabled = false;
        }
        else
        {
            PlayerPrefs.SetString("Music", "ON");
            GetComponent<Image>().sprite = musicOn;
            GameObject.Find("MainTheme").GetComponent<AudioSource>().enabled = true;
        }
    }

    public void PlayClickSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF")
            GetComponent<AudioSource>().Play();
    }

    private void PlayUnavailableSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF")
        {
        }
    }
}