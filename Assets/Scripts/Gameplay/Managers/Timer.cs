using UnityEngine;
using TMPro;
using Audio;
using Core.Constants;
using Gameplay.Spawning;
using UI;

namespace Gameplay.Managers
{
    public class Timer : MonoBehaviour
    {
        private enum WaveState
        {
            Spawning,
            Combat,
            Waiting,
            Choosing
        }

        GameStateManager GameStateManager => GameStateManager.Instance;
        Spawner Spawner => GameManager.Instance.Spawner;
        SoundManager Sound => GameManager.Instance.SoundManager;

        public int WaveNum;
        public int RoundNum;
        public int Addition = 0;

        [SerializeField] TMP_Text _text;
        [SerializeField] Color[] _colors;
        [SerializeField] private GameObject choicePrefab;
        
        private WaveState waveState;
        private float remainSpawnDelay;
        private float remainWaitingTime;
        private int remainingEnemies;
        private bool hasSpawnedEnemy;
        [SerializeField] private float spawnDelay = 0.5f;
        private const float ChoiceDelay = 0.25f;
        
        private void Start()
        {
            remainSpawnDelay = 0f;
            WaveNum = 1;
            RoundNum = 1;
            Addition = 0;
            BeginWave();
        }

        private void NextWave()
        {
            if (WaveNum == 7 && RoundNum == ConstantStore.BossPerWave)
            {
                GameManager.Instance.GameClear(0);
                return;
            }

            RoundNum++;
            if (RoundNum >= ConstantStore.BossPerWave + 1)
            {
                RoundNum -= ConstantStore.BossPerWave;
                WaveNum++;
            }

            BeginWave();
        }

        private void BeginWave()
        {
            if (RoundNum % ConstantStore.BossPerWave != 0)
            {
                // Normal
                SetText($"Wave {WaveNum}-{RoundNum}", _colors[0]);
                BeginSpawning(WaveNum * WaveNum + RoundNum + Addition);
                Sound.PlaySFX(SoundKey.Wave);
            }
            else
            {
                // Boss
                SetText($"Wave {WaveNum}-Boss", _colors[1]);
                BeginSpawning(WaveNum * WaveNum + 1);
                Sound.PlaySFX(SoundKey.Boss);
            }

            waveState = WaveState.Spawning;
        }

        private void BeginWaiting()
        {
            waveState = WaveState.Waiting;
            remainWaitingTime = ChoiceDelay;
            SetText("Wait...", _colors[2]);
            GameManager.Instance.Coins += 5;
        }

        private void ShowWaveChoice()
        {
            waveState = WaveState.Choosing;
            var choice = Instantiate(choicePrefab, Vector3.zero, Quaternion.identity).GetComponent<Choice>();
            choice.OnSelected = NextWave;
        }

        private void BeginSpawning(int enemyCount)
        {
            remainingEnemies = enemyCount;
            remainSpawnDelay = 0f;
            hasSpawnedEnemy = false;
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
                    hasSpawnedEnemy = true;
                }
                return;
            }

            waveState = WaveState.Combat;
        }

        private void UpdateCombat()
        {
            if (hasSpawnedEnemy && GameManager.Instance.ActiveEnemyNum == 0)
            {
                BeginWaiting();
            }
        }

        private void UpdateWaiting()
        {
            remainWaitingTime -= Time.deltaTime;
            if (remainWaitingTime <= 0f)
            {
                ShowWaveChoice();
            }
        }

    }
}
