using UnityEngine;
using TMPro;
using Starfall.Manager;

public class SynergyList : MonoBehaviour {
    #region Manager
    private static AbilityManager abilityManager => GameManager.Instance.AbilityManager;
    #endregion
    
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private string[] descriptions;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Start() {
        for(int i = 0; i < texts.Length; i++)
            texts[i].text = abilityManager.synergy[i+1].ToString() + "/" + abilityManager.synergy[i];
    }

    public void OnImageClick(int i) {
        descriptionText.text = descriptions[i];
    }
}