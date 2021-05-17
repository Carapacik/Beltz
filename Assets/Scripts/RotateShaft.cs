using UnityEngine;

public class RotateShaft : MonoBehaviour
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