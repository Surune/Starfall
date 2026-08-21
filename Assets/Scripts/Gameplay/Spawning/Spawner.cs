using System.Collections.Generic;
using UnityEngine;
using Audio;
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
    private SoundManager sound;

    [SerializeField] GameObject bossPrefab;
    [SerializeField] float boundary;
    [SerializeField] int enemyTypeNum;
    public float SpeedCoefficient = 1f;
    public GameObject EnemyList;
    public float MeteorCoefficient = 1f;
    public float AddHP = 0f;
    [SerializeField] EnemyData[] enemyDataList;
    [HideInInspector] public bool Disabled = false;
    [HideInInspector] public bool MakeMeteor = false;
    [HideInInspector] public bool SpawnRandom = false;
    private readonly List<Enemy> activeEnemies = new();
    private readonly List<Transform> activeTargets = new();
    const float maxX = 5f;
    const float maxY = 5f;

    public IReadOnlyList<Enemy> ActiveEnemies => activeEnemies;

    public Transform FindClosestTarget(Vector3 position)
    {
        Transform closest = null;
        var closestDistance = float.MaxValue;
        foreach (var target in activeTargets)
        {
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
        activeTargets.Remove(enemy.transform);
    }

    public void RemoveActiveTarget(Transform target)
    {
        activeTargets.Remove(target);
    }

    public void InjectDependency(GameDependencies dependencies)
    {
        this.dependencies = dependencies;
        poolManager = dependencies.PoolManager;
        timer = dependencies.Timer;
        sound = dependencies.SoundManager;
    }

    public GameObject SpawnMeteor()
    {
        if (Disabled)
        {
            return null;
        }
        
        sound.PlaySFX(SoundKey.Meteor);
        var meteor = poolManager.Spawn<Meteor>();
        meteor.transform.position = new Vector3(dependencies.Player.transform.position.x, maxY, 0f);
        meteor.speed *= MeteorCoefficient;
        return meteor.gameObject;
    }

    public GameObject SpawnFinalBoss()
    {
        foreach (var activeEnemy in activeEnemies)
        {
            activeEnemy.gameObject.SetActive(false);
        }
        activeEnemies.Clear();
        activeTargets.Clear();
        var enemy = Instantiate(bossPrefab, new Vector3(0f, maxY, 0f), Quaternion.identity);
        enemy.transform.SetParent(EnemyList.transform);
        activeTargets.Add(enemy.transform);
        enemy.GetComponent<Boss>().InjectDependency(dependencies);
        dependencies.EnemySpawned();
        return enemy;
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
        activeTargets.Add(e.transform);
        return e;
    }

    public void SpawnItem()
    {
        var item = poolManager.Spawn<DropItem>();
        item.transform.position = new Vector3(Random.Range(-maxX, maxX), maxY, 0f);
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

        var enemy = SpawnEnemyWithType(ran, new Vector3(Random.Range(-maxX, maxX), maxY, 0f));
        if (timer.RoundNum % ConstantStore.BossPerWave != 0)
        {
            enemy.IsBoss = false;
            enemy.ExpAmount = 1;
        }
        else
        {
            enemy.MakeBoss();
            enemy.ExpAmount = timer.WaveNum + 1;
        }
        enemy.MakeMeteor = MakeMeteor;
        enemy.MaxHP = enemy.MaxHP + AddHP > 1 ? enemy.MaxHP + AddHP : 1f;
        enemy.CurrentHP = enemy.MaxHP;
        dependencies.EnemySpawned();
    }
}
}
