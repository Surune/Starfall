using UnityEngine;
using Starfall.Manager;

[RequireComponent(typeof(ParticleSystem))]
public class Exp : MonoBehaviour {
    private static Transform player;
    private static ExpManager expmanager => GameManager.Instance.exp;
    private static int speed = 10;
    private ParticleSystem particle;
    private ParticleSystem.Particle[] particleList;
    public int exp_amount = 0;

    void Start() {
        if (player == null) 
            player = GameObject.Find("Player").transform;
        particle = GetComponent<ParticleSystem>();
        particleList = new ParticleSystem.Particle[particle.main.maxParticles];
    }

    void Update() {
        if (GameStateManager.Instance.IsPlaying) {
            int particleCount = particle.GetParticles(particleList);

            for (int i = 0; i < particleCount; i++)
                particleList[i].position = Vector3.Lerp(particleList[i].position, player.position, Time.smoothDeltaTime * speed);

            this.transform.position = Vector3.Lerp(transform.position, player.position, Time.smoothDeltaTime * speed);

            particle.SetParticles(particleList, particleCount);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag == "Player") {
            expmanager.GetExp(exp_amount);
            gameObject.SetActive(false);
        }
    }
}