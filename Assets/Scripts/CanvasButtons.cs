using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasButtons : MonoBehaviour
{
    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject soundIcon;

    private void Start()
    {
    }

    private void Update()
    {
    }



    public void NextScene()
    {
        PlayClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ChooseLevel()
    {
        PlayClickSound();
        SceneManager.LoadScene($"Lvl{GameObject.FindWithTag("LevelName").GetComponent<Text>().text}");
    }

    public void OpenSetting()
    {
        PlayClickSound();
        setting.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        setting.SetActive(false);
    }

    public void LevelMenu()
    {
        PlayClickSound();
        SceneManager.LoadScene("LevelMenu");
    }
    public void PlaySound()
    {
        if (PlayerPrefs.GetString("Sound") == "ON")
        {
            PlayerPrefs.SetString("Sound", "OFF");
            soundIcon.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetString("Sound", "ON");
            soundIcon.SetActive(false);
        }
    }
    
    public void PlayMusic()
    {
        if (PlayerPrefs.GetString("Music") == "ON")
        {
            PlayerPrefs.SetString("Music", "OFF");
            soundIcon.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetString("Music", "ON");
            soundIcon.SetActive(false);
        }
    }
    
    private void PlayClickSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF")
            GetComponent<AudioSource>().Play();
    }
}