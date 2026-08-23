using System.Threading.Tasks;
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
            Waiting
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
        private int remainingEnemies;
        [SerializeField] private float spawnDelay = 0.5f;
        
        private void Start()
        {
            remainSpawnDelay = 0f;
            WaveNum = 1;
            RoundNum = 0;
            Addition = 0;
            NextWave();
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

        private async Task BeginWaiting(float waitTime)
        {
            waveState = WaveState.Waiting;
            SetText("Wait...", _colors[2]);
            GameManager.Instance.Coins += 5;

            while (waitTime > 0f)
            {
                await Task.Yield();

                if (GameStateManager.IsPlaying)
                {
                    waitTime -= Time.deltaTime;
                }
            }
        }

        public void ShowChoice()
        {
            Instantiate(choicePrefab, Vector3.zero, Quaternion.identity);
        }

        private void ShowWaveChoice()
        {
            var choice = Instantiate(choicePrefab, Vector3.zero, Quaternion.identity).GetComponent<Choice>();
            choice.OnSelected = NextWave;
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
            }
        }

        private async void UpdateSpawning()
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
                await BeginWaiting(0.25f);
                ShowWaveChoice();
                return;
            }

            waveState = WaveState.Combat;
        }

        private async void UpdateCombat()
        {
            if (GameManager.Instance.ActiveEnemyNum == 0)
            {
                await BeginWaiting(0.25f);
                ShowWaveChoice();
            }
        }

    }
}
