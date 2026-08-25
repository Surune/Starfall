using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Audio;
using Core.Constants;
using Gameplay.Spawning;
using UI;
using Data.Abilities;
using Gameplay.Entities;

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
        [SerializeField] private Choice choicePrefab;
        
        private WaveState waveState;
        private float remainSpawnDelay;
        private float remainWaitingTime;
        private int remainingEnemies;
        private bool hasSpawnedEnemy;
        private readonly HashSet<uint> selectedPlayerIds = new();
        private int[] networkChoiceIndexes;
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
            if (WaveNum == 7 && RoundNum == Constants.BossPerWave)
            {
                GameManager.Instance.GameClear(0);
                return;
            }

            RoundNum++;
            if (RoundNum >= Constants.BossPerWave + 1)
            {
                RoundNum -= Constants.BossPerWave;
                WaveNum++;
            }

            BeginWave();
        }

        private void BeginWave()
        {
            if (RoundNum % Constants.BossPerWave != 0)
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
            ShowNetworkChoice();
        }

        private void ShowNetworkChoice()
        {
            networkChoiceIndexes = CreateNetworkChoiceIndexes();
            selectedPlayerIds.Clear();
            foreach (var connection in NetworkServer.connections.Values)
            {
                connection.identity.GetComponent<Player>().ServerShowChoice(networkChoiceIndexes);
            }
        }

        public void ShowNetworkChoice(AbilityData[] choices, Action<int> onSelected)
        {
            var choice = Instantiate(choicePrefab, Vector3.zero, Quaternion.identity);
            choice.InitializeNetworkChoice(choices, onSelected);
        }

        [Server]
        public void ServerSelectNetworkChoice(Player player, int choiceIndex)
        {
            var isValidChoice = false;
            for (var i = 0; i < networkChoiceIndexes.Length; i++)
            {
                if (choiceIndex == i)
                {
                    isValidChoice = true;
                    break;
                }
            }

            if (!isValidChoice)
            {
                return;
            }

            if (!selectedPlayerIds.Add(player.netId))
            {
                return;
            }

            if (selectedPlayerIds.Count != NetworkServer.connections.Count)
            {
                return;
            }

            GameStateManager.SetState(GameState.Gameplay);
            foreach (var connection in NetworkServer.connections.Values)
            {
                connection.identity.GetComponent<Player>().ServerResumeGameplay();
            }
            NextWave();
        }

        private int[] CreateNetworkChoiceIndexes()
        {
            var choices = new int[Constants.ChoiceCount];
            for (var i = 0; i < choices.Length; i++)
            {
                while (true)
                {
                    var ability = GameManager.Instance.AbilityManager.GetRandomAbility();
                    choices[i] = GameManager.Instance.AbilityManager.GetAbilityIndex(ability);
                    var isDuplicate = false;
                    for (var j = 0; j < i; j++)
                    {
                        if (choices[j] == choices[i])
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (!isDuplicate)
                    {
                        break;
                    }
                }
            }

            return choices;
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
            if (!NetworkServer.active)
            {
                return;
            }

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
