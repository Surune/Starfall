using UnityEngine;
using TMPro;
using Data.Abilities;
using Gameplay.Managers;

namespace UI
{
public class SynergyList : MonoBehaviour
{
    static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;

    [SerializeField] TMP_Text[] texts;
    [SerializeField] TMP_Text descriptionText;

    private void Start()
    {
        for (var i = 0; i < texts.Length; i++)
        {
            var synergyType = (AbilitySynergy)(i + 1);
            var synergy = AbilityManager.GetSynergyData(synergyType);
            texts[i].text = $"{AbilityManager.GetSynergyCount(synergyType)}/{synergy.Requirement}";
        }
    }

    public void OnImageClick(int i)
    {
        descriptionText.text = AbilityManager.GetSynergyData((AbilitySynergy)(i + 1)).Description;
    }
}
}
