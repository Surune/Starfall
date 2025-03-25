using UnityEngine;

namespace Starfall.Utils
{
    public class AutoInactivator : MonoBehaviour
    {
        [SerializeField] float _cooltime;

        void Start()
        {
            Invoke(nameof(InactiavteSelf), _cooltime);
        }

        void OnEnable()
        {
            Invoke(nameof(InactiavteSelf), _cooltime);
        }

        void InactiavteSelf()
        {
            gameObject.SetActive(false);
        }
    }
}
