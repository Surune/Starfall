using UnityEngine;
using TMPro;

namespace Starfall.Manager {
    public class ExpManager : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI resourceText;
        public float expCurrent = 0;
        public int expMax = 10;
        [SerializeField] private AudioSource musicPlayer;
        [SerializeField] private AudioClip sfxBonus;
        [SerializeField] private AudioClip sfxExp;
        [SerializeField] private GameObject choicePrefab;
        public int coins;
        [HideInInspector] public bool hextech = false;

        private void Start() {
            coins = 0;
            expCurrent = 0;
            expMax = 10;
            expCurrent += PlayerPrefs.GetInt("module_5");
            //SetText();
        }

        public void SetText() {
            resourceText.text = (int)expCurrent + "/" + expMax;
        }

        public void GetExp(int num) {
            expCurrent += num;

            if (expCurrent >= expMax) {
                if (hextech) GameManager.Instance.PlayerManager.refresh += 1;
                LevelUp();
            }
            else {
                musicPlayer.PlayOneShot(sfxExp);
            }
            SetText();
        }

        public void LevelUp() {
            GameStateManager.Instance.SetState(GameState.Paused);
            expCurrent -= expMax;
            expMax += 5;
            Instantiate(choicePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
            musicPlayer.PlayOneShot(sfxBonus);
            coins += 5;
            SetText();
        }
    }
}