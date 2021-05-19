using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("sound", "on");
        PlayerPrefs.SetString("music", "on");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
