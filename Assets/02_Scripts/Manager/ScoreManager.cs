using UnityEngine;
using TMPro;

namespace Starfall.Manager {
    public class ScoreManager : MonoBehaviour {
        public float totalScore = 0;
        
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private AudioSource musicPlayer;

        private void SetScoreText() {
            scoreText.text = "" + (int)totalScore;
        }

        public void GetScore(int num) {
            totalScore += num;
            PlayerPrefs.SetInt("NowScore", Mathf.RoundToInt(totalScore));

            SetScoreText();
        }
    }
}