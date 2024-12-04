using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Entity;

namespace Starfall.Manager {
    public class HPManager : MonoBehaviour {
        #region Manager
        private PlayerManager playerManager =>  GameManager.Instance.PlayerManager;
        private GameStateManager gameStateManager => GameManager.Instance.gameStateManager;
        private Player player => GameManager.Instance.Player;
        #endregion

        private Slider healthSlider;
        public float currentHP;
        public float maxHP;
        private AudioSource musicPlayer;
        public GameObject gameOverDisplay;
        public AudioClip sfxHit;
        public AudioClip sfxBarrier;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI barrierText;
        public int barrier = 0;
        [HideInInspector] public bool meatshield = false;
        [HideInInspector] public bool porcupine = false;
        [HideInInspector] public bool blunt = false;
        [HideInInspector] public bool our_l = false;
        [HideInInspector] public bool our_r = false;
        [HideInInspector] public bool lethal = false;
        [HideInInspector] public bool carving = false;
        [HideInInspector] public bool virgo = false;
        [HideInInspector] public bool berserk = false;
        [HideInInspector] public bool capricon = false;


        void Start () {
            healthSlider = GetComponent<Slider>();
            maxHP = 100f;
            currentHP = maxHP;
            barrier = 0;
            healthSlider.value = currentHP;
            musicPlayer = GameObject.Find("Player").GetComponent<AudioSource>();
            PlayerPrefs.SetFloat("nowdeck", 0f);
            barrierText = player.barrier.GetComponent<TextMeshProUGUI>();
        }

        public void SetHealthBar() {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
            hpText.text = currentHP + " / " + maxHP;
        }

        public void GetBarrier(int num) {
            barrier += num;
            player.barrier.SetActive(true);
            barrierText.text = "" + barrier;
        }

        public bool ChangeHP(int delta){
            if (currentHP + delta >= 0) {
                currentHP += delta;
                SetHealthBar();
                return true;
            }
            else {
                currentHP = 1;
                SetHealthBar();
                return false;
            }
        }

        public void GetDamage(int delta) {
            if (delta < 0) {
                if (barrier > 0) {
                    barrier -= 1;
                    barrierText.text = "" + barrier;
                    if (barrier == 0) {
                        player.barrier.SetActive(false);
                    }
                    if (meatshield) ChangeHP(+10);
                    if (blunt) playerManager.damage += 0.2f;
                    if (our_l) playerManager.criticalProb += 0.1f;
                    if (our_r) playerManager.criticalCoefficient += 0.1f;
                    if (carving) player.ChangeSkillCool(player.skillCooltimeMax * 0.9f);
                    if (capricon) GameManager.Instance.spawner.SpawnItem();
                    delta = 0;
                    musicPlayer.PlayOneShot(sfxBarrier);
                }
                else {
                    if (berserk) playerManager.damage += 0.05f;
                    if (virgo) delta = delta > 10 ? 10 : delta;
                    if (porcupine) playerManager.DamageAllEnemy(-delta);
                    musicPlayer.PlayOneShot(sfxHit);
                    if (lethal) currentHP = 0;
                }
            }
            currentHP += delta;
            if (currentHP >= maxHP) currentHP = maxHP;

            if (currentHP <= 0) {
                GameManager.Instance.GameOver(0);
                gameStateManager.SetState(GameState.Paused);
                Instantiate(gameOverDisplay, new Vector3(0f, 0f, 0f), Quaternion.identity);
            }
            SetHealthBar();
        }
    }
}