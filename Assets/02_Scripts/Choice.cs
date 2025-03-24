using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;
using Starfall.Entity;
using Starfall.Constants;

public class Choice : MonoBehaviour
{
    static Player Player => GameManager.Instance.Player;
    static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
    static ExpManager ExpManager => GameManager.Instance.ExpManager;
    static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
    static EffectManager EffectManager => GameManager.Instance.EffectManager;
    static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;

    public static bool Hardmode = false;
    static List<int> SelectedNumbers => GameManager.Instance.AbilityNumbers;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject refreshButton;
    [SerializeField] TextMeshProUGUI info;
    [SerializeField] ChoiceButton[] buttons;
    [SerializeField] int[] btnnums;
    [SerializeField] AudioClip sfxRefresh;
    [SerializeField] AudioClip sfxSelect;

    void Start()
    {
        btnnums = new int[buttons.Length];

        GameManager.Instance.ActiveChoiceNum += 1;
        GameStateManager.SetState(GameState.Paused);

        SetRefreshText();
        SetChoicenum();

        buttons[3].gameObject.SetActive(Hardmode);

        info.text = "현재까지 처치한 적 " + Player.KillNum
                    + "\n공격력 " + PlayerManager.damage + " / 공격력 계수 " + PlayerManager.damageCoefficient
                    + "\n연사 간격 " + Player.SkillCooltimeMax
                    + "\n치명타 확률 " + PlayerManager.criticalProb*100 + "% / 치명타 대미지 " + PlayerManager.criticalCoefficient
                    + "\n즉사탄 확률 " + PlayerManager.fatalProb*100 + "% / 고정 피해 " + PlayerManager.fixDamage;
    }

    void SetChoicenum()
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            while (true)
            {
                btnnums[i] = Random.Range(0, AbilityManager.AbilitySprites.Length);
                var isSame = SelectedNumbers.Contains(btnnums[i]);

                for (var j = 0; j < i; j++)
                {
                    if (btnnums[j] == btnnums[i])
                    {
                        isSame = true;
                        break;
                    }
                }

                if (!isSame)
                {
                    break;
                }
            }

            buttons[i].SetAbility(btnnums[i]);
        }
    }

    public void Clicked(int i)
    {
        AbilityManager.Choiced(btnnums[i]);
        GameManager.Instance.ActiveChoiceNum--;
        EffectManager.GetComponent<AudioSource>().PlayOneShot(sfxSelect);
        ExpManager.SetText();
        if (GameManager.Instance.ActiveChoiceNum == 0)
        {
            GameStateManager.SetState(GameState.Gameplay);
        }

        Destroy(canvas);
    }

    void SetRefreshText()
    {
        refreshButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "새로고침 (" + PlayerManager.refresh + "회 남음)";
        if (PlayerManager.refresh <= 0)
        {
            refreshButton.GetComponent<Button>().interactable = false;
        }
    }

    public void Refresh()
    {
        if (PlayerManager.refresh > 0)
        {
            PlayerManager.refresh -= 1;
            EffectManager.GetComponent<AudioSource>().PlayOneShot(sfxRefresh);
            SetRefreshText();
            SetChoicenum();
        }
    }
}
