using UnityEngine;
using Gameplay.Entities;

namespace Gameplay.Projectiles
{
    public sealed class ProjectileTargetResolver
    {
        public bool IsTarget(Collider2D collision)
        {
            return collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("Boss");
        }

        public bool IsObstacle(Collider2D collision)
        {
            return collision.transform.CompareTag("Obstacle");
        }

        public IDamageable GetDamageable(Collider2D collision)
        {
            return collision.gameObject.GetComponent<IDamageable>();
        }
    }
}
