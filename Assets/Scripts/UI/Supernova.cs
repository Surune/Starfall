using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Constants;

namespace UI
{
public class Supernova : MonoBehaviour
{
    public Button LeftButton; // 각 레벨 버튼을 저장할 배열
    public Button RightButton;
    public TMP_Text DescriptionText;
    public TMP_Text LevelText;

    int highestLevel;
    int currentLevel = 0;

    private void Start()
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

    private void SetText()
    {
        LevelText.text = $"SUPERNOVA\nCODE-{currentLevel}";
        if(currentLevel == 0)
        {
            DescriptionText.text = "NOTHING WILL HAPPEN";
        }
        else
        {
            DescriptionText.text = $"{Constants.NERF_TEXT_LIST[currentLevel-1]}\n클리어 시 코인, 점수 {5 * currentLevel}%";
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
}
