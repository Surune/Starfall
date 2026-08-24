using UnityEngine;
using Core.Constants;

namespace Gameplay.Entities
{
    public sealed class EnemyMovement
    {
        private Vector3 direction = Vector3.down;
        private float speed;

        public float MaxSpeed { get; set; } = 10f;
        public float SlowTime { get; private set; }

        public void Reset()
        {
            SlowTime = 0f;
        }

        public void Configure(EnemyType type, Camera camera, Vector3 position)
        {
            speed = MaxSpeed;
            direction = type == EnemyType.Blue && camera.WorldToViewportPoint(position).x < 0.5f
                ? new Vector3(0.5f, -1f, 0f)
                : type == EnemyType.Blue
                    ? new Vector3(-0.5f, -1f, 0f)
                    : Vector3.down;
        }

        public void MakeBoss()
        {
            MaxSpeed *= 0.5f;
            speed = MaxSpeed;
        }

        public void ApplySlow(float duration)
        {
            SlowTime = duration;
        }

        public bool Move(Transform transform, Camera camera, float deltaTime)
        {
            if (SlowTime > 0f)
            {
                SlowTime -= deltaTime;
                speed = MaxSpeed * 0.75f;
                if (SlowTime <= 0f)
                {
                    SlowTime = 0f;
                    speed = MaxSpeed;
                }
            }

            transform.Translate(direction * speed * deltaTime);

            var viewportPosition = camera.WorldToViewportPoint(transform.position);
            if (viewportPosition.y < 0f)
            {
                return false;
            }

            if (viewportPosition.x < 0f)
            {
                direction = new Vector3(0.5f, -1f, 0f);
            }
            else if (viewportPosition.x > 1f)
            {
                direction = new Vector3(-0.5f, -1f, 0f);
            }

            return true;
        }
    }
}
