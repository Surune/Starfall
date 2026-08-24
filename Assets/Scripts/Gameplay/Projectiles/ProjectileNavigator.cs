using UnityEngine;
using Gameplay.Spawning;

namespace Gameplay.Projectiles
{
    public sealed class ProjectileNavigator
    {
        private readonly Spawner spawner;

        public ProjectileNavigator(Spawner spawner)
        {
            this.spawner = spawner;
        }

        public void Move(Transform transform, bool isPlaying, float speed, bool isHoming)
        {
            if (!isPlaying)
            {
                return;
            }

            if (isHoming)
            {
                var closestTarget = spawner.FindClosestTarget(transform.position);
                if (closestTarget && Vector2.Distance(transform.position, closestTarget.position) < 1f)
                {
                    transform.position = Vector3.Lerp(transform.position, closestTarget.position, Time.smoothDeltaTime * speed);
                    return;
                }
            }

            transform.Translate(0f, Time.smoothDeltaTime * speed, 0f);
        }
    }
}
