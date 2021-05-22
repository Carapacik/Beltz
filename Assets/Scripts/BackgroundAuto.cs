using UnityEngine;

public class BackgroundAuto : MonoBehaviour
{
    [SerializeField] private GameObject parentCanvas;
    [SerializeField] private int backgroundNumber;

    private void Start()
    {
        if (backgroundNumber == 0) backgroundNumber = Random.Range(1, 7);
        var bg = Resources.Load<GameObject>($"Prefabs/BG/BG{backgroundNumber}").transform;
        Instantiate(bg, new Vector3(0, 0, 90), Quaternion.identity).SetParent(parentCanvas.transform);
    }
}