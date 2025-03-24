using UnityEngine;
using TMPro;

namespace Starfall.Manager
{
    public class ScoreManager : MonoBehaviour
    {
        public float TotalScore = 0;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] AudioSource musicPlayer;

        void SetScoreText()
        {
            scoreText.text = ((int)TotalScore).ToString();
        }

        public void GetScore(int num)
        {
            TotalScore += num;
            PlayerPrefs.SetInt("NowScore", Mathf.RoundToInt(TotalScore));

            SetScoreText();
        }
    }
}
