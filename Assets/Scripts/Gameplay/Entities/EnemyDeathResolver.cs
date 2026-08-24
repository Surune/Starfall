using System;
using UnityEngine;
using Core.Constants;
using Gameplay.Effects;
using Gameplay.Managers;
using Gameplay.Spawning;
using Random = UnityEngine.Random;

namespace Gameplay.Entities
{
    public sealed class EnemyDeathResolver
    {
        private readonly EffectManager effectManager;
        private readonly PoolManager poolManager;
        private readonly Player player;
        private readonly Spawner spawner;
        private readonly Timer timer;
        private readonly Action enemyRemoved;

        public EnemyDeathResolver(EffectManager effectManager, PoolManager poolManager, Player player, Spawner spawner, Timer timer, Action enemyRemoved)
        {
            this.effectManager = effectManager;
            this.poolManager = poolManager;
            this.player = player;
            this.spawner = spawner;
            this.timer = timer;
            this.enemyRemoved = enemyRemoved;
        }

        public bool TryResolve(Enemy enemy, EnemyType type, bool fatal, float itemProbability)
        {
            var defeatedByFatal = fatal && !enemy.IsBoss && enemy.gameObject.activeSelf;
            var defeatedByHealth = !defeatedByFatal && enemy.CurrentHealth <= 0f && enemy.gameObject.activeSelf;
            var isDead = defeatedByFatal || defeatedByHealth;
            if (!isDead)
            {
                return false;
            }

            if (defeatedByHealth && type == EnemyType.Green)
            {
                foreach (var activeEnemy in spawner.ActiveEnemies)
                {
                    if (activeEnemy != enemy)
                    {
                        activeEnemy.Heal(timer.WaveNum);
                    }
                }
            }
            else if (defeatedByHealth && type == EnemyType.Violet)
            {
                throw new NotImplementedException();
            }

            player.KillNum++;

            var effect = poolManager.Spawn<DamageEffect>();
            effect.transform.position = enemy.transform.position;
            effect.transform.localScale = enemy.transform.localScale;

            if (Random.Range(0, 100) < itemProbability)
            {
                var item = poolManager.Spawn<DropItem>();
                item.transform.position = enemy.transform.position;
                item.SetType((ItemType)Random.Range(0, 4));
            }

            Despawn(enemy);
            return true;
        }

        public void Despawn(Enemy enemy)
        {
            spawner.RemoveActiveEnemy(enemy);
            enemyRemoved();
            enemy.gameObject.SetActive(false);
        }
    }
}
