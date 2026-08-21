using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;

namespace Data.Abilities
{
    [CreateAssetMenu(menuName = "Starfall/Ability Database", fileName = "AbilityDatabase")]
    public class AbilityDatabase : ScriptableObject
    {
        public List<AbilityData> Abilities;
    }
}
