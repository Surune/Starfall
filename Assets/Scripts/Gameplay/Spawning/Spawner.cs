using System.Collections.Generic;
using UnityEngine;
using Core.Constants;
using Data.Enemies;
using Gameplay.Entities;
using Gameplay.Managers;

namespace Gameplay.Spawning
{
    public class Spawner : MonoBehaviour, IDependencyInjectable
    {
        private GameDependencies dependencies;
        private PoolManager poolManager;
        private Timer timer;

        [SerializeField] int enemyTypeNum;
        public float SpeedCoefficient = 1f;
        [SerializeField] EnemyData[] enemyDataList;
        [HideInInspector] public bool SpawnRandom = false;
        private readonly List<Enemy> activeEnemies = new();
        private const float MaxX = 5f;
        private const float MaxY = 5f;

        public IReadOnlyList<Enemy> ActiveEnemies => activeEnemies;

        public Transform FindClosestTarget(Vector3 position)
        {
            Transform closest = null;
            var closestDistance = float.MaxValue;
            foreach (var enemy in activeEnemies)
            {
                var target = enemy.transform;
                var distance = (target.position - position).sqrMagnitude;
                if (distance >= closestDistance)
                {
                    continue;
                }

                closest = target;
                closestDistance = distance;
            }

            return closest;
        }

        public void RemoveActiveEnemy(Enemy enemy)
        {
            activeEnemies.Remove(enemy);
        }

        public void InjectDependency(GameDependencies dependencies)
        {
            this.dependencies = dependencies;
            poolManager = dependencies.PoolManager;
            timer = dependencies.Timer;
        }

        public void SpawnItem()
        {
            var item = poolManager.Spawn<DropItem>();
            item.transform.position = new Vector3(Random.Range(-MaxX, MaxX), MaxY, 0f);
            item.SetType((ItemType)Random.Range(0, 4));
        }

        public void SpawnWaveEnemy()
        {
            int ran;
            if (SpawnRandom)
            {
                ran = Random.Range(0, enemyTypeNum);
            }
            else
            {
                ran = Random.Range(0, timer.WaveNum < enemyTypeNum ? timer.WaveNum : enemyTypeNum);
            }

            var enemy = SpawnEnemyWithType(ran, new Vector3(Random.Range(-MaxX, MaxX), MaxY, 0f));
            if (timer.RoundNum % ConstantStore.BossPerWave != 0)
            {
                enemy.IsBoss = false;
            }
            else
            {
                enemy.MakeBoss();
            }
            dependencies.EnemySpawned();
        }

        private Enemy SpawnEnemyWithType(int type, Vector3 pos)
        {
            var e = poolManager.Spawn<Enemy>();
            var enemyData = enemyDataList[type];
            e.transform.localPosition = pos;
            e.transform.localScale = Vector3.one;

            e.Maxspeed = enemyData.Speed * SpeedCoefficient;
            e.SetType(enemyData);
            activeEnemies.Add(e);
            return e;
        }
    }
}
