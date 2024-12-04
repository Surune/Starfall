using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;
using Starfall.Utils;
using Starfall.Entity;

public class ChoiceButton : MonoBehaviour {
    # region Manager
                        private static Player player => GameManager.Instance.Player;
                        private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
                        private static ExpManager expManager => GameManager.Instance.exp;
                        private static AbilityManager abilityManager => GameManager.Instance.AbilityManager;
                        private static EffectManager effectManager => GameManager.Instance.EffectManager;
                        private static GameStateManager gameStateManager => GameManager.Instance.gameStateManager;
    [SerializeField]    private GameObject canvas;
    [SerializeField]    private GameObject refreshButton;
    [SerializeField]    private TextMeshProUGUI info;
    [SerializeField]    private Transform[] buttons;
    # endregion

    # region Property
    [SerializeField]    private int[] btnnums;
    [SerializeField]    private List<int> selectedNumbers => GameManager.Instance.AbilityNumbers;
    [SerializeField]    private AudioClip sfxRefresh;
    [SerializeField]    private AudioClip sfxSelect;
                        public static bool hardmode = false;
    # endregion

    void Start () {
        btnnums = new int[buttons.Length];
        
        GameManager.Instance.activeChoiceNum += 1;
        gameStateManager.SetState(GameState.Paused);
        
        SetRefreshText();
        TweenButtons();
        SetChoicenum();
        
        if (!hardmode)
            buttons[3].gameObject.SetActive(false);

        info.text = "현재까지 처치한 적 " + player.killNum
                    + "\n공격력 " + playerManager.damage + " / 공격력 계수 " + playerManager.damageCoefficient
                    + "\n연사 간격 " + player.skillCooltimeMax
                    + "\n치명타 확률 " + playerManager.criticalProb*100 + "% / 치명타 대미지 " + playerManager.criticalCoefficient
                    + "\n즉사탄 확률 " + playerManager.fatalProb*100 + "% / 고정 피해 " + playerManager.fixDamage;
    }

    private void SetChoicenum() {
        for (int i = 0; i < buttons.Length; i++) {
            while (true) {
                btnnums[i] = Random.Range(0, abilityManager.abilitySprites.Length);
                bool isSame = false;
                if (selectedNumbers.Contains(btnnums[i])) isSame = true;

                for (int j = 0; j < i; j++) {
                    if (btnnums[j] == btnnums[i]) {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }

            int rannum = btnnums[i];
            buttons[i].GetChild(0).gameObject.GetComponent<Image>().sprite = abilityManager.abilitySprites[rannum];
            SetSynergyIcon(buttons[i], rannum);
            buttons[i].GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = abilityManager.abilityList[rannum]["NAME"].ToString();
            buttons[i].GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = abilityManager.abilityList[rannum]["DESCRIPTION"].ToString();
        }
    }

    private void SetSynergyIcon(Transform button, int num) {
        int s1 = int.Parse(abilityManager.abilityList[num]["SYNERGY1"].ToString());
        if (s1 == 0) {
            button.GetChild(1).gameObject.SetActive(false);
        }
        else {
            button.GetChild(1).gameObject.SetActive(true);
            button.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().sprite = abilityManager.SynergySprites[s1];
            button.GetChild(1).gameObject.GetComponent<Image>().color = abilityManager.SynergyColors[s1];
        }
        
        int s2 = int.Parse(abilityManager.abilityList[num]["SYNERGY2"].ToString());
        if (s2 == 0) {
            button.GetChild(2).gameObject.SetActive(false);
        }
        else {
            button.GetChild(2).gameObject.SetActive(true);
            button.GetChild(2).GetChild(0).gameObject.GetComponent<Image>().sprite = abilityManager.SynergySprites[s2];
            button.GetChild(2).gameObject.GetComponent<Image>().color = abilityManager.SynergyColors[s2];
        }
    }
    
    public void Clicked(int i) {
        abilityManager.Choiced(btnnums[i]);
        GameManager.Instance.activeChoiceNum--;
        effectManager.GetComponent<AudioSource>().PlayOneShot(sfxSelect);
        expManager.SetText();
        if (GameManager.Instance.activeChoiceNum == 0)
            gameStateManager.SetState(GameState.Gameplay);
        
        Destroy(canvas);
    }

    private void TweenButtons() {
        foreach(var t in buttons)
            t.gameObject.GetComponent<Tween>().tween();
    }

    private void SetRefreshText() {
        refreshButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "새로고침 (" + playerManager.refresh + "회 남음)";
        if(playerManager.refresh <= 0)
            refreshButton.GetComponent<Button>().interactable = false;
    }

    public void Refresh() {
        if(playerManager.refresh > 0) {
            playerManager.refresh -= 1;
            effectManager.GetComponent<AudioSource>().PlayOneShot(sfxRefresh);
            SetRefreshText();
            TweenButtons();
            SetChoicenum();
        }
    }
}
