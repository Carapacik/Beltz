using UnityEngine;

public class BackgroundChoose : MonoBehaviour
{
    [SerializeField] private GameObject parentCanvas;
    [SerializeField] private int backgroundNumber;

    private void Start()
    {
        if (backgroundNumber == 0) backgroundNumber = Random.Range(1, 9);
        var bg = Resources.Load<GameObject>($"Prefabs/Backgrounds/BG{backgroundNumber}").transform;
        Instantiate(bg, new Vector3(0, 0, 90), Quaternion.identity).SetParent(parentCanvas.transform);
    }
}