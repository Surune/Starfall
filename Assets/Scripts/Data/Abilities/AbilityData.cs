using System;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;
using Gameplay.Entities;

namespace Data.Abilities
{
    public enum AbilityStat
    {
        Damage = 0,
        DamageCoefficient = 1,
        CriticalProbability = 2,
        CriticalCoefficient = 3,
        FatalProbability = 4,
        ShotSpeedCoefficient = 6,
        SkillCooldown = 7,
        MaxHealth = 8,
        CurrentHealth = 9,
        Barrier = 10,
        EnemyDamageCoefficient = 11,
        EnemyItemProbability = 12,
        EnemySpeedCoefficient = 14,
        Refresh = 18,
        CoinCoefficient = 19,
        BulletCount = 21,
        WingCount = 22
    }

    public enum AbilityModifierOperation
    {
        Add,
        Multiply,
        RestoreToMaximum
    }

    public enum AbilityType
    {
        Passive,
        Active
    }

    public enum AbilitySynergy
    {
        None,
        BurningStrike,
        InfestingStrike,
        Swarmer,
        HealingStrike,
        Insurance,
        HomingBarrage,
        SilencingStrike,
        KineticStrike,
        WhispersOfDoom,
        Wing
    }

    [Serializable]
    public class AbilityModifier
    {
        public AbilityStat Stat;
        public AbilityModifierOperation Operation;
        public float Value;
    }

    [CreateAssetMenu(menuName = "Starfall/Ability Data", fileName = "AbilityData")]
    public class AbilityData : ScriptableObject
    {
        public string Name;
        [TextArea] public string Description;
        public Sprite Icon;
        public AbilitySynergy Synergy1;
        public AbilitySynergy Synergy2;
        public List<AbilityModifier> Modifiers;
        public AbilityType Type;
        public float Cooldown;
    }
}
