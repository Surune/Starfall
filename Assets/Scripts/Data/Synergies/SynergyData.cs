using System.Collections.Generic;
using UnityEngine;
using Data.Abilities;

namespace Data.Synergies
{
    public enum SynergyEffect
    {
        None,
        DisableSpawner,
        EnableStatikk,
        SetCriticalProbabilityToOne
    }

    [CreateAssetMenu(menuName = "Starfall/Synergy Data", fileName = "SynergyData")]
    public class SynergyData : ScriptableObject
    {
        public AbilitySynergy Type;
        public string Name;
        [TextArea] public string Description;
        public Sprite Icon;
        public Color Color;
        public int Requirement;
        public List<AbilityModifier> Modifiers;
        public SynergyEffect Effect;
    }
}
