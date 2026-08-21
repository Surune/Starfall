using UnityEngine;
using TMPro;

namespace UI
{
public class Store : MonoBehaviour
{
    [SerializeField] private TMP_Text ResourceText;
    public int Coins;

    private void Start()
    {
        Coins = PlayerPrefs.GetInt("TotalCoin");
        SetText();
    }

    private void SetText()
    {
        Coins = PlayerPrefs.GetInt("TotalCoin");
        SetCoins(Coins);
    }

    public void SetCoins(int coins)
    {
        Coins = coins;
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
}
