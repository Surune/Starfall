using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Constants;

namespace Starfall.Manager
{
    public class Timer : MonoBehaviour
    {
        Spawner spawner => GameManager.Instance.Spawner;

        public int WaveNum;
        public int RoundNum;
        public bool Waiting = false;
        public float WaitTimeMax = 0f;
        public int Addition = 0;

        [SerializeField] AudioSource MusicPlayer;
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] float _timeMax;
        [SerializeField] GameObject _enemyList;
        [SerializeField] Color[] _colors;
        [SerializeField] Image _fillarea;
        [SerializeField] AudioClip _sfxWave;
        [SerializeField] AudioClip _sfxBoss;
        [SerializeField] AudioClip _sfxFinalBoss;
        float waitTimeNow;
        float remainTime;

        GameStateManager GameStateManager => GameStateManager.Instance;

        void Start()
        {
            waitTimeNow = 0f;
            remainTime = _timeMax;
            WaveNum = 1;
            RoundNum = 0;
            Addition = 0;
            NextWave();
        }

        void NextWave()
        {
            RoundNum++;
            if (RoundNum >= ConstantStore.BossPerWave + 1)
            {
                RoundNum -= ConstantStore.BossPerWave;
                WaveNum++;
            }

            if (WaveNum == 8)
            {
                if (RoundNum == 1)
                {
                    spawner.SpawnFinalBoss();
                    SetText("SOMETHING BIG IS COMING...!", _colors[2]);
                    MusicPlayer.PlayOneShot(_sfxFinalBoss);
                    RoundNum = 2;
                }
            }
            else
            {
                Waiting = false;
                if (RoundNum % ConstantStore.BossPerWave != 0)
                {
                    // Normal
                    SetText($"Wave {WaveNum}-{RoundNum}", _colors[0]);
                    remainTime = _timeMax;
                    spawner.Enemynum = WaveNum * WaveNum + RoundNum + Addition;
                    MusicPlayer.PlayOneShot(_sfxWave);
                }
                else
                {
                    // Boss
                    SetText($"Wave {WaveNum}-Boss", _colors[1]);
                    remainTime = _timeMax + WaveNum;
                    spawner.Enemynum = WaveNum * WaveNum + 1;
                    MusicPlayer.PlayOneShot(_sfxBoss);
                }
            }
        }

        void SetText(string t, Color c)
        {
            _text.text = t;
            _text.color = c;
        }

        void Update()
        {
            if (!GameStateManager.IsPlaying)
            {
                return;
            }

            if (Waiting)
            {
                waitTimeNow -= Time.deltaTime;
                if (waitTimeNow <= 0 && WaveNum != 8)
                {
                    NextWave();
                }
                return;
            }

            if (remainTime <= 0)
            {
                Waiting = true;
                SetText("Wait...", _colors[2]);
                waitTimeNow = WaitTimeMax;
            }
            else if (spawner.Enemynum == 0)
            {
                if (GameManager.Instance.ActiveEnemyNum == 0)
                {
                    // if time is left and all enemy killed
                    Waiting = true;
                    SetText("Wait...", _colors[2]);
                    waitTimeNow = 0.25f;
                }
                else
                {
                    remainTime -= Time.deltaTime;
                }
            }
        }
    }
}
