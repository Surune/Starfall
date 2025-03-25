using UnityEngine;
using Starfall.Manager;
using Starfall.Entity;

[RequireComponent(typeof(ParticleSystem))]
public class Exp : MonoBehaviour
{
    static Player Player => GameManager.Instance.Player;
    static ExpManager Expmanager => GameManager.Instance.ExpManager;
    static readonly int Speed = 10;
    ParticleSystem particle;
    ParticleSystem.Particle[] particleList;
    public int ExpAmount = 0;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particleList = new ParticleSystem.Particle[particle.main.maxParticles];
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPlaying)
        {
            int particleCount = particle.GetParticles(particleList);

            for (int i = 0; i < particleCount; i++)
            {
                particleList[i].position = Vector3.Lerp(particleList[i].position, Player.transform.position, Time.smoothDeltaTime * Speed);
            }

            transform.position = Vector3.Lerp(transform.position, Player.transform.position, Time.smoothDeltaTime * Speed);

            particle.SetParticles(particleList, particleCount);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Expmanager.GetExp(ExpAmount);
            gameObject.SetActive(false);
        }
    }
}
