using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Levels : MonoBehaviour
{
    public void Update()
    {
        var lastLvl = PlayerPrefs.GetInt("LastCompletedLevel");
        var levelItems = GameObject.FindGameObjectsWithTag("LevelItem");
        foreach (var item in levelItems)
        {
            var lvlName = item.transform.GetChild(1).transform;
            if (lastLvl >= int.Parse(lvlName.GetComponent<TextMeshProUGUI>().text))
            {
                item.transform.GetChild(0).transform.GetComponent<Image>().color = 
                    new Color(1, 1, 1, 0.9f);
                lvlName.GetComponent<TextMeshProUGUI>().color =
                    new Color(0.1254361f, 0.82f, 0.2322076f, 0.9f);
            }
        }
    }
}