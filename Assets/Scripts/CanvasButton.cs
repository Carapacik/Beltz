using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButton : MonoBehaviour
{
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private AudioClip errorAudioSource;

    private void Start()
    {
        switch (PlayerPrefs.GetString("Sound"))
        {
            case "":
                PlayerPrefs.SetString("Sound", "ON");
                break;
            case "OFF" when gameObject.name == "SoundButton":
                GetComponent<Image>().sprite = soundOff;
                break;
        }

        switch (PlayerPrefs.GetString("Music"))
        {
            case "":
                PlayerPrefs.SetString("Music", "ON");
                break;
            case "OFF" when gameObject.name == "MusicButton":
                GetComponent<Image>().sprite = musicOff;
                break;
        }

        if (PlayerPrefs.GetInt("LastCompletedLevel") == 16) PlayerPrefs.SetInt("LastCompletedLevel", 0);
        PlayerPrefs.Save();
    }

    public void Play()
    {
        if (PlayerPrefs.GetInt("LastCompletedLevel") == 0 && PlayerPrefs.GetInt("HighestLevel") == 0)
            SceneManager.LoadScene("Tutorial");
        else ChooseLevel(PlayerPrefs.GetInt("LastCompletedLevel") + 1);
    }

    public void ChooseLevel(int number)
    {
        if (number > PlayerPrefs.GetInt("HighestLevel") + 1)
        {
            GetComponent<AudioSource>().clip = errorAudioSource;
            if (PlayerPrefs.GetString("Sound") != "OFF")
                GetComponent<AudioSource>().Play();
        }
        else if (number is < 17 and > 0)
        {
            PlayClickSound();
            SceneManager.LoadScene($"Lvl{number}");
        }
        else
        {
            PlayClickSound();
            SceneManager.LoadScene("Complete");
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

    public void RedirectToGit()
    {
        Application.OpenURL("https://github.com/Carapacik/Beltz");
    }

    public void PlayClickSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF") GetComponent<AudioSource>().Play();
    }
}