using UnityEngine;

public class RotateBall : MonoBehaviour
{
    [SerializeField] private float rotation;

    private void Start()
    {
    }

    private void Update()
    {
        transform.Rotate(0, 0, -rotation);
    }
}