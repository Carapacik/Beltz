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
            0 => Random.Range(1, 16),
            _ => backgroundNumber
        };
        var bg = Resources.Load<Image>($"BG/BG{backgroundNumber}");
        Instantiate(bg, parentCanvas.transform).transform.SetSiblingIndex(0);
    }
}