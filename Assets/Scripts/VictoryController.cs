using System.Linq;
using UnityEngine;

public class VictoryController : MonoBehaviour
{
    [SerializeField] private GameObject victoryObject;
    [SerializeField] private int currentLvl;
    private GameObject[] _belts;

    private void Start()
    {
        _belts = GameObject.FindGameObjectsWithTag("Belt");
    }

    private void Update()
    {
        var correctCounter = _belts.Select(belt => belt.GetComponent<SwapBelts>())
            .Count(x => x.isCorrect);
        if (correctCounter == _belts.Length)
        {
            victoryObject.SetActive(true);
            var shafts = GameObject.Find("Shafts").transform;
            foreach (Transform shaft in shafts)
                shaft.gameObject.GetComponent<RotateShaft>().enabled = true;
            foreach (var belt in _belts)
                belt.gameObject.GetComponent<SwapBelts>().enabled = false;
            PlayerPrefs.SetInt("LastCompletedLevel", currentLvl);
            if (currentLvl > PlayerPrefs.GetInt("HighestLevel"))
                PlayerPrefs.SetInt("HighestLevel", currentLvl);
            PlayerPrefs.Save();
        }
    }
}