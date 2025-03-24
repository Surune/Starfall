using Starfall.Manager;
using Starfall.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    AbilityManager abilityManager => GameManager.Instance.AbilityManager;

    int abilityId = -1;
    [SerializeField] Image abilityIcon;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image synergy1Background;
    [SerializeField] Image synergy1Icon;
    [SerializeField] Image synergy2Background;
    [SerializeField] Image synergy2Icon;
    [SerializeField] Tween tween;

    public void SetAbility(int id)
    {
        abilityId = id;
        abilityIcon.sprite = abilityManager.AbilitySprites[id];
        nameText.text = abilityManager.AbilityList[id]["NAME"].ToString();
        descriptionText.text = abilityManager.AbilityList[id]["DESCRIPTION"].ToString();

        var first = int.Parse(abilityManager.AbilityList[id]["SYNERGY1"].ToString());
        if (first == 0)
        {
            synergy1Background.gameObject.SetActive(false);
        }
        else
        {
            synergy1Background.gameObject.SetActive(true);
            synergy1Background.color = abilityManager.SynergyColors[first];
            synergy1Icon.sprite = abilityManager.SynergySprites[first];
        }

        var second = int.Parse(abilityManager.AbilityList[id]["SYNERGY2"].ToString());
        if (second == 0)
        {
            synergy2Background.gameObject.SetActive(false);
        }
        else
        {
            synergy2Background.gameObject.SetActive(true);
            synergy2Background.color = abilityManager.SynergyColors[second];
            synergy2Icon.sprite = abilityManager.SynergySprites[second];
        }

        tween.DoTween();
    }
}
