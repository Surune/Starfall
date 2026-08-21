using UnityEngine;
using Gameplay.Entities;
using Gameplay.Managers;

namespace Gameplay.Collectibles
{
public class Exp : MonoBehaviour
{
    static Player Player => GameManager.Instance.Player;
    static ExpManager Expmanager => GameManager.Instance.ExpManager;
    [SerializeField] private ParticleSystem particle;
    ParticleSystem.Particle[] particleList;
    public int ExpAmount = 0;
    private const int Speed = 10;

    private void Start()
    {
        particleList = new ParticleSystem.Particle[particle.main.maxParticles];
    }

    public void Update()
    {
        if (!GameStateManager.Instance.IsPlaying)
        {
            return;
        }
        
        var particleCount = particle.GetParticles(particleList);
        for (var i = 0; i < particleCount; i++)
        {
            particleList[i].position = Vector3.Lerp(particleList[i].position, Player.transform.position, Time.smoothDeltaTime * Speed);
        }

        transform.position = Vector3.Lerp(transform.position, Player.transform.position, Time.smoothDeltaTime * Speed);

        particle.SetParticles(particleList, particleCount);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Expmanager.GetExp(ExpAmount);
            gameObject.SetActive(false);
        }
    }
}
}
