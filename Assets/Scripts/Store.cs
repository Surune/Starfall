using UnityEngine;
using TMPro;
using Starfall.Manager;

public class Store : MonoBehaviour
{
    public TextMeshProUGUI ResourceText;
    public int Coins;

    void Start()
    {
        Coins = PlayerPrefs.GetInt("TotalCoin");
        ResourceText = GameObject.Find("Coin").GetComponent<TextMeshProUGUI>();
        SetText();
    }

    public void SetText()
    {
        Coins = PlayerPrefs.GetInt("TotalCoin");
        ResourceText.text = $"Coins : {Coins}";
    }

    public void DestroyOnClick()
    {
        Destroy(gameObject);
    }

    public void InstantiateOnClick(GameObject prefab)
    {
        Instantiate(prefab, transform);
    }
}
