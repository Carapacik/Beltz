using UnityEngine;

public class BackgroundChoose : MonoBehaviour
{
    [SerializeField] private GameObject parentCanvas;
    [SerializeField] private int backgroundNumber;

    private void Start()
    {
        backgroundNumber = backgroundNumber switch
        {
            -1 => PlayerPrefs.GetInt("LastCompletedLevel") + 1,
            0 => Random.Range(1, 13),
            _ => backgroundNumber
        };
        var bg = Resources.Load<GameObject>($"Prefabs/Backgrounds/BG{backgroundNumber}").transform;
        Instantiate(bg, new Vector3(0, 0, 90), Quaternion.identity).SetParent(parentCanvas.transform);
    }
}