using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterVictory : MonoBehaviour
{
    public void OnClose()
    {
        var currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneBuildIndex == 13)
            SceneManager.LoadScene("LevelMenu");
        else
            SceneManager.LoadScene(currentSceneBuildIndex + 1);
    }
}