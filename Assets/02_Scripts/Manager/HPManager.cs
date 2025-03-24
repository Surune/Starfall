using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Entity;
using Starfall.Constants;

namespace Starfall.Manager
{
    public class HPManager : MonoBehaviour
    {
        PlayerManager PlayerManager =>  GameManager.Instance.PlayerManager;
        GameStateManager GameStateManager => GameManager.Instance.GameStateManager;
        Player Player => GameManager.Instance.Player;

        Slider _healthSlider;
        AudioSource MusicPlayer => Player.MusicPlayer;
        public float CurrentHP;
        public float MaxHP;
        public GameObject GameOverDisplay;
        public AudioClip SfxHit;
        public AudioClip SfxBarrier;
        [SerializeField] TextMeshProUGUI _barrierText;
        public int Barrier = 0;
        [HideInInspector] public bool Meatshield = false;
        [HideInInspector] public bool Porcupine = false;
        [HideInInspector] public bool Blunt = false;
        [HideInInspector] public bool Our_l = false;
        [HideInInspector] public bool Our_r = false;
        [HideInInspector] public bool Lethal = false;
        [HideInInspector] public bool Carving = false;
        [HideInInspector] public bool Virgo = false;
        [HideInInspector] public bool Berserk = false;
        [HideInInspector] public bool Capricon = false;

        void Start()
        {
            _healthSlider = GetComponent<Slider>();
            MaxHP = 100f;
            CurrentHP = MaxHP;
            Barrier = 0;
            _healthSlider.value = CurrentHP;
            PlayerPrefs.SetFloat("nowdeck", 0f);
        }

        public void SetHealthBar()
        {
            _healthSlider.maxValue = MaxHP;
            _healthSlider.value = CurrentHP;
        }

        public void GetBarrier(int num)
        {
            Barrier += num;
            Player.Barrier.SetActive(true);
            _barrierText.text = "" + Barrier;
        }

        public bool ChangeHP(int delta)
        {
            if (CurrentHP + delta >= 0)
            {
                CurrentHP += delta;
                SetHealthBar();
                return true;
            }
            else
            {
                CurrentHP = 1;
                SetHealthBar();
                return false;
            }
        }

        public void GetDamage(int delta)
        {
            if (delta < 0)
            {
                if (Barrier > 0)
                {
                    Barrier -= 1;
                    _barrierText.text = "" + Barrier;
                    if (Barrier == 0)
                    {
                        Player.Barrier.SetActive(false);
                    }
                    if (Meatshield)
                    {
                        ChangeHP(+10);
                    }
                    if (Blunt)
                    {
                        PlayerManager.damage += 0.2f;
                    }
                    if (Our_l)
                    {
                        PlayerManager.criticalProb += 0.1f;
                    }
                    if (Our_r)
                    {
                        PlayerManager.criticalCoefficient += 0.1f;
                    }
                    if (Carving)
                    {
                        Player.ChangeSkillCool(Player.SkillCooltimeMax * 0.9f);
                    }
                    if (Capricon)
                    {
                        GameManager.Instance.Spawner.SpawnItem();
                    }
                    delta = 0;
                    MusicPlayer.PlayOneShot(SfxBarrier);
                }
                else
                {
                    if (Berserk)
                    {
                        PlayerManager.damage += 0.05f;
                    }
                    if (Virgo)
                    {
                        delta = delta > 10 ? 10 : delta;
                    }
                    if (Porcupine)
                    {
                        PlayerManager.DamageAllEnemy(-delta);
                    }
                    MusicPlayer.PlayOneShot(SfxHit);
                    if (Lethal)
                    {
                        CurrentHP = 0;
                    }
                }
            }
            CurrentHP += delta;
            if (CurrentHP >= MaxHP)
            {
                CurrentHP = MaxHP;
            }

            if (CurrentHP <= 0)
            {
                GameManager.Instance.GameOver(0);
                GameStateManager.SetState(GameState.Paused);
                Instantiate(GameOverDisplay, new Vector3(0f, 0f, 0f), Quaternion.identity);
            }
            SetHealthBar();
        }
    }
}
