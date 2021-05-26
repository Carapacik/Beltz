using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButtons : MonoBehaviour
{
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private AudioClip errorSound;

    private void Start()
    {
        var sound = PlayerPrefs.GetString("Sound");
        if (sound == "") PlayerPrefs.SetString("Sound", "ON");
        if (sound == "OFF" && sound != "" && gameObject.name == "SoundButton")
            GetComponent<Image>().sprite = soundOff;

        var music = PlayerPrefs.GetString("Music");
        if (music == "") PlayerPrefs.SetString("Music", "ON");
        if (music == "OFF" && gameObject.name == "MusicButton")
            GetComponent<Image>().sprite = musicOff;

        if (PlayerPrefs.GetInt("HighestLevel") == 0) PlayerPrefs.SetInt("HighestLevel", 1);
        PlayerPrefs.Save();
    }

    public void Play()
    {
        // TODO : Last completed level
        ChooseLevel(PlayerPrefs.GetInt("HighestLevel"));
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

    private void PlayErrorSound()
    {
        GetComponent<AudioSource>().clip = errorSound;
        if (PlayerPrefs.GetString("Sound") != "OFF")
            GetComponent<AudioSource>().Play();
    }
}