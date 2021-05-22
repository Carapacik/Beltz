using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterVictory : MonoBehaviour
{
    public void OnClose()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}