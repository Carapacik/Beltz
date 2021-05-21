using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    [SerializeField] private GameObject parentCanvas;

    private void Start()
    {
        var bg = Resources.Load<GameObject>($"Prefabs/BG/BG{Random.Range(1, 7)}").transform;
        Instantiate(bg, new Vector3(0, 0, 90), Quaternion.identity).SetParent(parentCanvas.transform);
    }
}