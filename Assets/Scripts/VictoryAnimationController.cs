using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryAnimationController : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF") GetComponent<AudioSource>().Play();
    }

    private void NextLevel()
    {
        var currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Lvl12")
        {
            SceneManager.LoadScene(currentScene.buildIndex + 1);
        }
        else if (PlayerPrefs.GetInt("Secret") > 1)
        {
            SceneManager.LoadScene("LvlSecret");
        }
        else
        {
            var secretCounter = PlayerPrefs.GetInt("Secret") + 1;
            PlayerPrefs.SetInt("Secret", secretCounter);
            LevelMenu();
        }
    }

    private void LevelMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}