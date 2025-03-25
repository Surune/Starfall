using UnityEngine;
using TMPro;
using Starfall.Manager;
using UnityEngine.Serialization;

public class SynergyList : MonoBehaviour
{
    static AbilityManager AbilityManager => GameManager.Instance.AbilityManager;

    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] string[] descriptions;
    [SerializeField] TextMeshProUGUI descriptionText;

    void Start()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = $"{AbilityManager.Synergy[i+1].ToString()}/{AbilityManager.Synergy[i]}";
        }
    }

    public void OnImageClick(int i)
    {
        descriptionText.text = descriptions[i];
    }
}
