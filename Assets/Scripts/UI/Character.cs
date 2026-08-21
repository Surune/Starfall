using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Constants;

namespace UI
{
public class Character : MonoBehaviour
{
    public static int CurrentPilot;
    public TMP_Text CharacterName;
    public TMP_Text Description;
    public Image Image;

    string[] NameList => ConstantStore.CHARACTER_NAME_LIST;
    string[] DescriptionList => ConstantStore.CHARACTER_DESCRIPTION_LiST;

    public Color[] ColorList;

    void Start()
    {
        SetText();
    }

    public void SetText()
    {
        CurrentPilot = PlayerPrefs.GetInt("currentPilot", 1);
        CharacterName.text = NameList[CurrentPilot-1];
        Description.text = DescriptionList[CurrentPilot-1];
        Image.color = ColorList[CurrentPilot-1];
    }
}
}
