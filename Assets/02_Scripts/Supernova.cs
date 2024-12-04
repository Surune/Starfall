using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;
using Starfall.Constants;

public class Supernova : MonoBehaviour {
    public Button leftButton; // 각 레벨 버튼을 저장할 배열
    public Button rightButton;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;
    
    private int highestLevel;
    private int currentLevel = 0;

    private void Start() {
        highestLevel = PlayerPrefs.GetInt("highestLevel", 0);
        if (highestLevel == 0) {
            PlayerPrefs.SetInt("currentLevel", 0);
            leftButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
        }
        else {
            currentLevel = highestLevel;
            PlayerPrefs.SetInt("currentLevel", currentLevel);
            rightButton.interactable = false;  
        }
        SetText();
    }

    public void SetText() {
        levelText.text = $"SUPERNOVA\nCODE-{currentLevel}";
        if(currentLevel == 0) {
            descriptionText.text = "NOTHING WILL HAPPEN";
        }
        else {
            descriptionText.text = $"{ConstantStore.NERF_TEXT_LIST[currentLevel-1]}\n클리어 시 코인, 점수 {5 * currentLevel}%";
        }
    }

    public void LeftClicked() {
        currentLevel -= 1;
        rightButton.interactable = true;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        if (currentLevel == 0) {
            leftButton.interactable = false;
        }
        SetText();
    }

    public void RightClicked() {
        currentLevel += 1;
        leftButton.interactable = true;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        if (currentLevel == highestLevel) {
            rightButton.interactable = false;
        }
        SetText();
    }
}
