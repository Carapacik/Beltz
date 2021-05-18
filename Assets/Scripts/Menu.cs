using UnityEngine;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var randNumber = Random.Range(1, 5);
        var bg = Resources.Load($"Prefabs/BG/BG{randNumber}");
        Instantiate(bg);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}