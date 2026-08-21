using UnityEngine;
using TMPro;

namespace UI
{
public class GameOver : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] AudioClip sfxEmerge;

    private void Start()
    {
        scoreText.text = $"Coins : {PlayerPrefs.GetInt("TotalCoin")} (+{PlayerPrefs.GetInt("Coin")})";
    }
}
}
