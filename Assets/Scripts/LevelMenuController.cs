using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuController : MonoBehaviour
{
    public void Update()
    {
        var highLvl = PlayerPrefs.GetInt("HighestLevel");
        var levelItems = GameObject.FindGameObjectsWithTag("LevelItem");
        foreach (var item in levelItems)
        {
            var lvlName = item.transform.GetChild(1).transform;
            if (highLvl >= int.Parse(lvlName.GetComponent<TextMeshProUGUI>().text))
            {
                item.transform.GetChild(0).transform.GetComponent<Image>().color = new Color(1, 1, 1, .9f);
                lvlName.GetComponent<TextMeshProUGUI>().color = new Color(.3f, .85f, .5f, .9f);
            }
        }
    }
}