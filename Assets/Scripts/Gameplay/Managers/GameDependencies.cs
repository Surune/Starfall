using System;
using Audio;
using Gameplay.Entities;
using Gameplay.Spawning;

namespace Gameplay.Managers
{
    public class GameDependencies
    {
        public AbilityManager AbilityManager { get; }
        public EffectManager EffectManager { get; }
        public GameStateManager GameStateManager { get; }
        public HPManager HPManager { get; }
        public Player Player { get; }
        public PlayerManager PlayerManager { get; }
        public PoolManager PoolManager { get; }
        public SoundManager SoundManager { get; }
        public Spawner Spawner { get; }
        public Timer Timer { get; }
        public Action EnemySpawned { get; }
        public Action EnemyRemoved { get; }

        public GameDependencies(AbilityManager abilityManager, EffectManager effectManager, GameStateManager gameStateManager, HPManager hpManager, Player player, PlayerManager playerManager, PoolManager poolManager, SoundManager soundManager, Spawner spawner, Timer timer, Action enemySpawned, Action enemyRemoved)
        {
            AbilityManager = abilityManager;
            EffectManager = effectManager;
            GameStateManager = gameStateManager;
            HPManager = hpManager;
            Player = player;
            PlayerManager = playerManager;
            PoolManager = poolManager;
            SoundManager = soundManager;
            Spawner = spawner;
            Timer = timer;
            EnemySpawned = enemySpawned;
            EnemyRemoved = enemyRemoved;
        }
    }
}
