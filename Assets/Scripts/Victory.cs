using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public GameObject victoryUI;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] belts = GameObject.FindGameObjectsWithTag("belt");
        var counter = 0;
        foreach (var belt in belts) {
            Swap c = belt.GetComponent<Swap>();

            if (c._isCorrect) {
                counter++;
            }
        }

        if (counter == belts.Length)
        {
            victoryUI.SetActive(true);
        }
    }
}
