using UnityEngine;

public class LoadSceneAfterVictory : MonoBehaviour
{
    public void OnClose()
    {
        CanvasButtons.NextScene();
    }
}