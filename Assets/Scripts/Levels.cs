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
            var lvlName = item.transform.GetChild(1);
            var text = lvlName.transform.GetComponent<TextMeshProUGUI>().text;

            if (lastLvl < int.Parse(text))
            {
                item.transform.GetChild(0).transform.GetComponent<Image>().color = new Color(0, 0, 0, (float) 0.4);
                lvlName.gameObject.SetActive(false);
            }
        }
    }
}