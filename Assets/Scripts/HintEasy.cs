using UnityEngine;

public class HintEasy : MonoBehaviour
{
    public void ShowHint()
    {
        var belts = GameObject.FindGameObjectsWithTag("Belt");
        foreach (var belt in belts)
        {
            var correct = belt.gameObject.GetComponent<SwapBelts>().isCorrect;
            if (!correct)
            {
                var animator = belt.gameObject.GetComponent<Animator>();
                animator.SetTrigger("Hint");
                break;
            }
        }
    }
}