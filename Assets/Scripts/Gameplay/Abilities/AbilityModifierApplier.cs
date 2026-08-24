using System;
using System.Collections.Generic;
using UnityEngine;
using Data.Abilities;
using Gameplay.Entities;
using Gameplay.Managers;
using Gameplay.Spawning;

namespace Gameplay.Abilities
{
    public sealed class AbilityModifierApplier
    {
        private readonly Dictionary<AbilityStat, Action<AbilityModifier>> handlers;

        public AbilityModifierApplier(Player player, HPManager hpManager, Spawner spawner, PlayerManager playerManager, GameManager gameManager)
        {
            handlers = new Dictionary<AbilityStat, Action<AbilityModifier>>
            {
                [AbilityStat.Damage] = modifier => Apply(ref playerManager.damage, modifier),
                [AbilityStat.DamageCoefficient] = modifier => Apply(ref playerManager.damageCoefficient, modifier),
                [AbilityStat.CriticalProbability] = modifier => Apply(ref playerManager.criticalProb, modifier),
                [AbilityStat.CriticalCoefficient] = modifier => Apply(ref playerManager.criticalCoefficient, modifier),
                [AbilityStat.FatalProbability] = modifier => Apply(ref playerManager.fatalProb, modifier),
                [AbilityStat.ShotSpeedCoefficient] = modifier => Apply(ref playerManager.shotSpeedCoefficient, modifier),
                [AbilityStat.SkillCooldown] = modifier => ApplySkillCooldown(player, modifier),
                [AbilityStat.MaxHealth] = modifier => ApplyMaxHealth(hpManager, modifier),
                [AbilityStat.CurrentHealth] = modifier => ApplyCurrentHealth(hpManager, modifier),
                [AbilityStat.Barrier] = modifier => hpManager.GetBarrier(Mathf.RoundToInt(modifier.Value)),
                [AbilityStat.EnemyDamageCoefficient] = modifier => Apply(ref Enemy.DamageCoefficient, modifier),
                [AbilityStat.EnemyItemProbability] = modifier => Apply(ref Enemy.ItemProb, modifier),
                [AbilityStat.EnemySpeedCoefficient] = modifier => Apply(ref spawner.SpeedCoefficient, modifier),
                [AbilityStat.Refresh] = modifier => playerManager.refresh += Mathf.RoundToInt(modifier.Value),
                [AbilityStat.CoinCoefficient] = modifier => gameManager.CoinCoefficient += modifier.Value,
                [AbilityStat.BulletCount] = modifier => Apply(ref player.BulletCount, modifier),
                [AbilityStat.WingCount] = modifier => playerManager.GetWing(Mathf.RoundToInt(modifier.Value))
            };
        }

        public void Apply(AbilityModifier modifier)
        {
            handlers[modifier.Stat](modifier);
        }

        private static void ApplySkillCooldown(Player player, AbilityModifier modifier)
        {
            var cooldown = player.SkillCooltimeMax;
            Apply(ref cooldown, modifier);
            player.ChangeSkillCool(cooldown);
        }

        private static void ApplyMaxHealth(HPManager hpManager, AbilityModifier modifier)
        {
            Apply(ref hpManager.MaxHP, modifier);
            hpManager.SetHealthBar();
        }

        private static void ApplyCurrentHealth(HPManager hpManager, AbilityModifier modifier)
        {
            if (modifier.Operation == AbilityModifierOperation.RestoreToMaximum)
            {
                hpManager.CurrentHP = hpManager.MaxHP;
            }
            else
            {
                Apply(ref hpManager.CurrentHP, modifier);
            }

            hpManager.SetHealthBar();
        }

        private static void Apply(ref float stat, AbilityModifier modifier)
        {
            if (modifier.Operation == AbilityModifierOperation.Add)
            {
                stat += modifier.Value;
                return;
            }

            stat *= modifier.Value;
        }

        private static void Apply(ref int stat, AbilityModifier modifier)
        {
            if (modifier.Operation == AbilityModifierOperation.Add)
            {
                stat += Mathf.RoundToInt(modifier.Value);
                return;
            }

            stat = Mathf.RoundToInt(stat * modifier.Value);
        }
    }
}
