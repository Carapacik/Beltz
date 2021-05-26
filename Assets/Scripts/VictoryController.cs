using System.Linq;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    [SerializeField] private GameObject victoryObject;
    [SerializeField] private int currentLvl;

    private void Update()
    {
        var belts = GameObject.FindGameObjectsWithTag("Belt");
        var correctCounter = belts.Select(belt => belt.GetComponent<SwapBelts>())
            .Count(x => x.isCorrect);
        if (correctCounter == belts.Length)
        {
            victoryObject.SetActive(true);

            var shafts = GameObject.Find("Shafts").transform;
            foreach (Transform shaft in shafts)
                shaft.gameObject.GetComponent<RotateShaft>().enabled = true;
            foreach (var belt in belts)
                belt.gameObject.GetComponent<SwapBelts>().enabled = false;

            PlayerPrefs.SetInt("LastCompletedLevel", currentLvl);
            if (currentLvl > PlayerPrefs.GetInt("HighestLevel")) 
                PlayerPrefs.SetInt("HighestLevel", currentLvl);
            PlayerPrefs.Save();
        }
    }
}