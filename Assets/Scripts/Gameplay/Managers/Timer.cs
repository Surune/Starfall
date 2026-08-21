using UnityEngine;
using TMPro;
using Audio;
using Core.Constants;
using Gameplay.Entities;
using Gameplay.Spawning;

namespace Gameplay.Managers
{
    public class Timer : MonoBehaviour
    {
        private enum WaveState
        {
            Spawning,
            Combat,
            Waiting,
            FinalBoss
        }

        GameStateManager GameStateManager => GameStateManager.Instance;
        Spawner Spawner => GameManager.Instance.Spawner;
        SoundManager Sound => GameManager.Instance.SoundManager;

        public int WaveNum;
        public int RoundNum;
        public int Addition = 0;

        [SerializeField] TMP_Text _text;
        [SerializeField] Color[] _colors;
        
        private WaveState waveState;
        private float remainWaitTime;
        private float remainWaveTime;
        private float remainSpawnDelay;
        private int remainingEnemies;
        private const float maxWaitTime = 3f;
        private const float waveTime = 3f;
        [SerializeField] private float spawnDelay = 0.5f;
        
        private void Start()
        {
            remainWaitTime = 0f;
            remainWaveTime = waveTime;
            remainSpawnDelay = 0f;
            WaveNum = 1;
            RoundNum = 0;
            Addition = 0;
            NextWave();
        }

        private void NextWave()
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
                    Spawner.SpawnFinalBoss();
                    SetText("SOMETHING BIG IS COMING...!", _colors[2]);
                    Sound.PlaySFX(SoundKey.FinalBoss);
                    RoundNum = 2;
                    waveState = WaveState.FinalBoss;
                }
            }
            else
            {
                if (RoundNum % ConstantStore.BossPerWave != 0)
                {
                    // Normal
                    SetText($"Wave {WaveNum}-{RoundNum}", _colors[0]);
                    remainWaveTime = waveTime;
                    BeginSpawning(WaveNum * WaveNum + RoundNum + Addition);
                    Sound.PlaySFX(SoundKey.Wave);
                }
                else
                {
                    // Boss
                    SetText($"Wave {WaveNum}-Boss", _colors[1]);
                    remainWaveTime = waveTime + WaveNum;
                    BeginSpawning(WaveNum * WaveNum + 1);
                    Sound.PlaySFX(SoundKey.Boss);
                }

                waveState = WaveState.Spawning;
            }
        }

        private void BeginWaiting(float waitTime)
        {
            waveState = WaveState.Waiting;
            SetText("Wait...", _colors[2]);
            remainWaitTime = waitTime;
        }

        private void BeginSpawning(int enemyCount)
        {
            remainingEnemies = enemyCount;
            remainSpawnDelay = 0f;
        }

        private void SetText(string t, Color c)
        {
            _text.text = t;
            _text.color = c;
        }

        private void Update()
        {
            if (!GameStateManager.IsPlaying)
            {
                return;
            }

            switch (waveState)
            {
                case WaveState.Spawning:
                    UpdateSpawning();
                    break;
                case WaveState.Combat:
                    UpdateCombat();
                    break;
                case WaveState.Waiting:
                    UpdateWaiting();
                    break;
            }
        }

        private void UpdateSpawning()
        {
            if (remainingEnemies > 0)
            {
                remainSpawnDelay -= Time.deltaTime;
                if (remainSpawnDelay <= 0f)
                {
                    Spawner.SpawnWaveEnemy();
                    remainingEnemies--;
                    remainSpawnDelay = spawnDelay;
                }
                return;
            }

            if (GameManager.Instance.ActiveEnemyNum == 0)
            {
                BeginWaiting(0.25f);
                return;
            }

            waveState = WaveState.Combat;
        }

        private void UpdateCombat()
        {
            if (GameManager.Instance.ActiveEnemyNum == 0)
            {
                BeginWaiting(0.25f);
                return;
            }

            remainWaveTime -= Time.deltaTime;
            if (remainWaveTime <= 0)
            {
                BeginWaiting(maxWaitTime);
            }
        }

        private void UpdateWaiting()
        {
            remainWaitTime -= Time.deltaTime;
            if (remainWaitTime <= 0)
            {
                NextWave();
            }
        }
    }
}
