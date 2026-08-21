
using Gameplay.Collectibles;
using Gameplay.Entities;

namespace Audio
{
    public enum SoundKey
    {
        None = 0,
        // 0~99 : UI
        Select = 1,
        Fail = 2,
        Purchase = 3,
        MoveScene = 4,
        // 100~199 : Ingame
        Item = 100,
        Synergy = 101,
        Levelup = 102,
        Exp = 103,
        Shoot = 104,
        Upgrade = 105,
        Barrier = 106,
        Hit = 107,
        Refresh = 110,
        Wave = 111,
        Boss = 112,
        FinalBoss = 113,
        Meteor = 114,
        // 200~201 : Enemy
        EnemyHit = 200,
        EnemyCritical = 201,
        EnemyKilled = 202,
    }
    
    public enum SoundType
    {
        Bgm,
        Effect,
        MaxCount
    }
}
