using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Data.Abilities;
using Gameplay.Managers;

namespace UI
{
    public class ChoiceButton : MonoBehaviour
    {
        AbilityManager abilityManager => GameManager.Instance.AbilityManager;

        [SerializeField] private Button button;
        [SerializeField] Image abilityIcon;
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] Image synergy1Background;
        [SerializeField] Image synergy1Icon;
        [SerializeField] Image synergy2Background;
        [SerializeField] Image synergy2Icon;

        public void Initialize(AbilityData ability, int index, Action<int> onClick)
        {
            button.onClick.AddListener(() => onClick(index));
            abilityIcon.sprite = ability.Icon;
            nameText.text = ability.Name;
            descriptionText.text = ability.Description;

            var first = ability.Synergy1;
            if (first == AbilitySynergy.None)
            {
                synergy1Background.gameObject.SetActive(false);
            }
            else
            {
                synergy1Background.gameObject.SetActive(true);
                var synergy = abilityManager.GetSynergyData(first);
                synergy1Background.color = synergy.Color;
                synergy1Icon.sprite = synergy.Icon;
            }

            var second = ability.Synergy2;
            if (second == AbilitySynergy.None)
            {
                synergy2Background.gameObject.SetActive(false);
            }
            else
            {
                synergy2Background.gameObject.SetActive(true);
                var synergy = abilityManager.GetSynergyData(second);
                synergy2Background.color = synergy.Color;
                synergy2Icon.sprite = synergy.Icon;
            }
        }
    }
}
