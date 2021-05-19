using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    public GameObject setting;
    public GameObject soundIcon;
    private void Start()
    {

    }

    private void Update()
    {
    }

    public void SoundSwitch()
    {
        
        if(PlayerPrefs.GetString("sound") == "on")
        {
            PlayerPrefs.SetString("sound", "off");
            soundIcon.SetActive(true);
        } else
        {
            PlayerPrefs.SetString("sound", "on");
            soundIcon.SetActive(false);
        }
    }

    public void NextScene()
    {
        PlaySound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenSetting()
    {
        setting.SetActive(true);
        PlaySound();
    }

    public void CloseSettings()
    {
        setting.SetActive(false);
        PlaySound();
    }

    public void LevelMenu()
    {
        PlaySound();
        SceneManager.LoadScene("LevelMenu");
    }
    private void PlaySound()
    {
        if (PlayerPrefs.GetString("sound") != "off")
            GetComponent<AudioSource>().Play();
    }
}