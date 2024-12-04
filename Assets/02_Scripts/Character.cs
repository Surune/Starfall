using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Constants;

public class Character : MonoBehaviour {
    public Image image;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI description;
    public static int currentPilot;

    private string[] nameList => ConstantStore.CHARACTER_NAME_LIST;
    private string[] descriptionList => ConstantStore.CHARACTER_DESCRIPTION_LiST;
    
    public Color[] colorList;

    private void Start() {
        SetText();
    }

    public void SetText() {
        currentPilot = PlayerPrefs.GetInt("currentPilot", 1);
        characterName.text = nameList[currentPilot-1];
        //description.text = desclist[currentPilot-1];
        image.color = colorList[currentPilot-1];
    }
}
