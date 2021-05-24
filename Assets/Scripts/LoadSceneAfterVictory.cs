using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterVictory : MonoBehaviour
{
    public void OnClose()
    {
        CanvasButtons.NextScene();
    }
}