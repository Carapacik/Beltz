using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private GameObject parentCanvas;
    [SerializeField] private int backgroundNumber;

    private void Start()
    {
        backgroundNumber = backgroundNumber switch
        {
            -1 => PlayerPrefs.GetInt("LastCompletedLevel") + 1,
            0 => Random.Range(1, 12),
            _ => backgroundNumber
        };
        var bg = Resources.Load<Image>($"BG/BG{backgroundNumber}");
        Instantiate(bg, parentCanvas.transform).transform.SetSiblingIndex(0);
    }
}