using System;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using Core.Constants;
using Data.Abilities;
using Gameplay.Managers;

namespace UI
{
    public class Choice : MonoBehaviour
    {
        public Action OnSelected = () => { };
        private Action<int> networkChoiceSelected = _ => { };
        private bool isNetworkChoice;
        
        AbilityManager AbilityManager => GameManager.Instance.AbilityManager;
        SoundManager Sound => GameManager.Instance.SoundManager;
        GameStateManager GameStateManager => GameManager.Instance.GameStateManager;

        static List<AbilityData> SelectedAbilities => GameManager.Instance.SelectedAbilities;
        [SerializeField] private Transform choiceContent;
        [SerializeField] private ChoiceButton choiceButtonPrefab;
        [SerializeField] private AbilityData[] abilityChoices;

        public int ChoiceCount => Constants.ChoiceCount;

        public void InitializeNetworkChoice(AbilityData[] choices, Action<int> onSelected)
        {
            isNetworkChoice = true;
            abilityChoices = choices;
            networkChoiceSelected = onSelected;
        }

        private void Start()
        {
            GameStateManager.SetState(GameState.Paused);
            Sound.PlaySFX(SoundKey.Choice);

            if (isNetworkChoice)
            {
                SetChoices();
            }
            else
            {
                abilityChoices = new AbilityData[ChoiceCount];
                SetChoicenum();
            }
        }

        private void SetChoicenum()
        {
            for (var i = 0; i < ChoiceCount; i++)
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

            }

            SetChoices();
        }

        private void SetChoices()
        {
            for (var i = 0; i < ChoiceCount; i++)
            {
                var button = Instantiate(choiceButtonPrefab, choiceContent);
                button.Initialize(abilityChoices[i], i, OnButtonClick);
            }
        }

        private void OnButtonClick(int i)
        {
            if (isNetworkChoice)
            {
                networkChoiceSelected(i);
                Destroy(gameObject);
                return;
            }

            AbilityManager.Choiced(abilityChoices[i]);
            Sound.PlaySFX(SoundKey.Select);
            GameStateManager.SetState(GameState.Gameplay);
            OnSelected();

            Destroy(gameObject);
        }
    }
}
