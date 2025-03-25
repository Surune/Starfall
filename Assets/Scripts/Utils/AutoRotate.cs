using UnityEngine;

namespace Starfall.Utils
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField] bool _fixSpeed;
        [SerializeField] float _rotateSpeed;

        void Start()
        {
            if (!_fixSpeed)
            {
                _rotateSpeed = Random.value * 50f;
            }
            if (Random.value > 0.5f)
            {
                _rotateSpeed = -_rotateSpeed;
            }
        }

        void Update()
        {
            transform.Rotate(_rotateSpeed * Time.deltaTime * Vector3.forward);
        }
    }
}
