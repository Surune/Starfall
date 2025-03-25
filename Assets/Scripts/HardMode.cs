using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HardMode : MonoBehaviour
{
    public Toggle Button;
    public TextMeshProUGUI ResourceText;
    public GameObject Description;

    bool cleared;
    bool hardmode;

    void Start()
    {
        cleared = System.Convert.ToBoolean(PlayerPrefs.GetInt("highestLevel", 0));
        hardmode = System.Convert.ToBoolean(PlayerPrefs.GetInt("hardMode", 0));
        if (hardmode)
        {
            Button.isOn = true;
            ResourceText.text = "HARD MODE";
            Description.SetActive(true);
        }
    }

    public void OnValueChanged()
    {
        if(Button.isOn)
        {
            if (cleared)
            {
                ResourceText.text = "HARD MODE";
                Description.SetActive(true);
                PlayerPrefs.SetInt("hardMode", 1);
            }
            else
            {
                Button.isOn = false;
                Description.SetActive(true);
                Description.GetComponent<TextMeshProUGUI>().text = "1회 클리어 시 개방";
                PlayerPrefs.SetInt("hardMode", 0);
            }
        }
        else
        {
            ResourceText.text = "NORMAL MODE";
            Description.SetActive(false);
            PlayerPrefs.SetInt("hardMode", 0);
        }
        PlayerPrefs.Save();
    }
}
