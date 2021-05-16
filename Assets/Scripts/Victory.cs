using System.Linq;
using UnityEngine;

public class Victory : MonoBehaviour
{
    [SerializeField] private GameObject victoryUI;

    private void Start()
    {
    }

    public void Update()
    {
        var belts = GameObject.FindGameObjectsWithTag("belt");
        var counter = belts.Select(belt => belt.GetComponent<Swap>()).Count(c => c.isCorrect);
        if (counter == belts.Length)
        {
            victoryUI.SetActive(true);
            var balls = GameObject.Find("Balls").transform;
            foreach (Transform ball in balls) ball.gameObject.GetComponent<RotateBall>().enabled = true;
        }
    }
}