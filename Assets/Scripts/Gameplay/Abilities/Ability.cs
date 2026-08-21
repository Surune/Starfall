using UnityEngine;
using UnityEngine.UI;
using Data.Abilities;
using Gameplay.Managers;

namespace Gameplay.Abilities
{
public class Ability : MonoBehaviour
{
    [SerializeField] private Image abilityIcon;

    private AbilityType abilityType;
    private AbilityData abilityData;
    private float cooldown;
    private float activeElapsed;

    public void Configure(AbilityData ability, Color iconColor)
    {
        abilityType = ability.Type;
        abilityData = ability;
        cooldown = ability.Cooldown;
        activeElapsed = 0f;
        abilityIcon.sprite = ability.Icon;
        abilityIcon.fillAmount = abilityType == AbilityType.Passive ? 1f : 0f;
        abilityIcon.color = iconColor;
    }

    private void Update()
    {
        if (abilityType == AbilityType.Passive || !GameManager.Instance.GameStateManager.IsPlaying)
        {
            return;
        }

        activeElapsed += Time.deltaTime;
        abilityIcon.fillAmount = Mathf.Clamp01(activeElapsed / cooldown);
        if (activeElapsed < cooldown)
        {
            return;
        }

        activeElapsed = 0f;
        GameManager.Instance.AbilityManager.ApplyModifiers(abilityData.Modifiers);
    }
}
}
