using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Audio;
using Core.Constants;
using Data.Abilities;
using Gameplay.Entities;
using Gameplay.Managers;

namespace UI
{
public class Choice : MonoBehaviour
{
    public Action OnSelected = () => { };

    Player Player => GameManager.Instance.Player;
    PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
    AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
    SoundManager Sound => GameManager.Instance.SoundManager;
    GameStateManager GameStateManager => GameManager.Instance.GameStateManager;

    static List<AbilityData> SelectedAbilities => GameManager.Instance.SelectedAbilities;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject refreshButton;
    [SerializeField] TMP_Text info;
    [SerializeField] ChoiceButton[] buttons;
    [SerializeField] AbilityData[] abilityChoices;

    private void Start()
    {
        abilityChoices = new AbilityData[buttons.Length];
        
        GameStateManager.SetState(GameState.Paused);
        Sound.PlaySFX(SoundKey.Choice);

        SetRefreshText();
        SetChoicenum();

        info.text = "현재까지 처치한 적 " + Player.KillNum
                    + "\n공격력 " + PlayerManager.damage + " / 공격력 계수 " + PlayerManager.damageCoefficient
                    + "\n연사 간격 " + Player.SkillCooltimeMax
                    + "\n치명타 확률 " + PlayerManager.criticalProb*100 + "% / 치명타 대미지 " + PlayerManager.criticalCoefficient
                    + "\n즉사탄 확률 " + PlayerManager.fatalProb*100 + "%";
    }

    private void SetChoicenum()
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            while (true)
            {
                abilityChoices[i] = AbilityManager.GetRandomAbility();
                var isSame = SelectedAbilities.Contains(abilityChoices[i]);

                for (var j = 0; j < i; j++)
                {
                    if (abilityChoices[j] == abilityChoices[i])
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

            buttons[i].SetAbility(abilityChoices[i]);
        }
    }

    public void Clicked(int i)
    {
        AbilityManager.Choiced(abilityChoices[i]);
        Sound.PlaySFX(SoundKey.Select);
        GameStateManager.SetState(GameState.Gameplay);
        OnSelected();

        Destroy(canvas);
    }

    private void SetRefreshText()
    {
        refreshButton.transform.GetChild(0).GetComponent<TMP_Text>().text = $"새로고침 {PlayerManager.refresh}회 남음)";
        if (PlayerManager.refresh <= 0)
        {
            refreshButton.GetComponent<Button>().interactable = false;
        }
    }

    public void Refresh()
    {
        if (PlayerManager.refresh <= 0)
        {
            return;
        }
        
        PlayerManager.refresh -= 1;
        Sound.PlaySFX(SoundKey.Refresh);
        SetRefreshText();
        SetChoicenum();
    }
}
}
