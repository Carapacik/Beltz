using UnityEngine;

public class RotateShaft : MonoBehaviour
{
    [SerializeField] private float rotation;
    [SerializeField] private GameObject belt;

    private void FixedUpdate()
    {
        if (belt != null)
        {
            if (belt.GetComponent<SwapBelts>().isCorrect) transform.Rotate(0, 0, -rotation);
        }
        else
        {
            transform.Rotate(0, 0, -rotation);
        }
    }
}