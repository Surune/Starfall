using UnityEngine;
using TMPro;
using Starfall.Manager;

public class Store : MonoBehaviour {
    public TextMeshProUGUI resourceText;
    public int coins;

    void Start() {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().ApplyMute();
        coins = PlayerPrefs.GetInt("TotalCoin");
        resourceText = GameObject.Find("Coin").GetComponent<TextMeshProUGUI>();
        SetText();
    }

    public void SetText() {
        coins = PlayerPrefs.GetInt("TotalCoin");
        resourceText.text = "Coins : " + coins;
    }

    public void DestroyOnClick() {
        //SetText();
        Destroy(this.gameObject);
    }

    public void InstantiateOnClick(GameObject prefab) {
        Instantiate(prefab, this.transform);
    }
}
