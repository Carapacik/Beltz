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
        var currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneBuildIndex == 13) LevelMenu();
        else SceneManager.LoadScene(currentSceneBuildIndex + 1);
    }

    private void LevelMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}