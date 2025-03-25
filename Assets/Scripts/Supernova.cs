using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;
using Starfall.Constants;

public class Supernova : MonoBehaviour
{
    public Button LeftButton; // 각 레벨 버튼을 저장할 배열
    public Button RightButton;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI LevelText;

    int highestLevel;
    int currentLevel = 0;

    void Start()
    {
        highestLevel = PlayerPrefs.GetInt("highestLevel", 0);
        if (highestLevel == 0)
        {
            PlayerPrefs.SetInt("currentLevel", 0);
            LeftButton.gameObject.SetActive(false);
            RightButton.gameObject.SetActive(false);
        }
        else
        {
            currentLevel = highestLevel;
            PlayerPrefs.SetInt("currentLevel", currentLevel);
            RightButton.interactable = false;
        }
        SetText();
    }

    void SetText()
    {
        LevelText.text = $"SUPERNOVA\nCODE-{currentLevel}";
        if(currentLevel == 0)
        {
            DescriptionText.text = "NOTHING WILL HAPPEN";
        }
        else
        {
            DescriptionText.text = $"{ConstantStore.NERF_TEXT_LIST[currentLevel-1]}\n클리어 시 코인, 점수 {5 * currentLevel}%";
        }
    }

    public void LeftClicked()
    {
        currentLevel -= 1;
        RightButton.interactable = true;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        if (currentLevel == 0)
        {
            LeftButton.interactable = false;
        }
        SetText();
    }

    public void RightClicked()
    {
        currentLevel += 1;
        LeftButton.interactable = true;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        if (currentLevel == highestLevel)
        {
            RightButton.interactable = false;
        }
        SetText();
    }
}
