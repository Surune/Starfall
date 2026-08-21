using System.Collections.Generic;
using UnityEngine;
using Core.Constants;
using Gameplay.Entities;

namespace Data.Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Starfall/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public EnemyType Type;
        public float BaseHP;
        public float StageHP;
        public float Speed;
        public List<Sprite> Sprites;
    }
}
