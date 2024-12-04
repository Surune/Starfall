using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HardMode : MonoBehaviour {
    public Toggle btn;
    public TextMeshProUGUI resourceText;
    public GameObject description;
    private bool cleared;
    private bool hardmode;

    private void Start() {
        cleared = System.Convert.ToBoolean(PlayerPrefs.GetInt("highestLevel", 0));
        hardmode = System.Convert.ToBoolean(PlayerPrefs.GetInt("hardMode", 0));
        if (hardmode){
            btn.isOn = true;
            resourceText.text = "HARD MODE";
            description.SetActive(true);
        }
    }

    public void onValueChanged(){
        if(btn.isOn) {
            if (cleared) {
                resourceText.text = "HARD MODE";
                description.SetActive(true);
                PlayerPrefs.SetInt("hardMode", 1);
            }
            else {
                btn.isOn = false;
                description.SetActive(true);
                description.GetComponent<TextMeshProUGUI>().text = "1회 클리어 시 개방";
                PlayerPrefs.SetInt("hardMode", 0);
            }
        }
        else {
            resourceText.text = "NORMAL MODE";
            description.SetActive(false);
            PlayerPrefs.SetInt("hardMode", 0);
        }
        PlayerPrefs.Save();
    }
}
