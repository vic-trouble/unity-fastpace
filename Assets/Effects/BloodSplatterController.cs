using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterController : MonoBehaviour
{
    private struct PosAndRotation
    {
        public Vector3 position;
        public Quaternion rotation;

        public PosAndRotation(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    private List<PosAndRotation> particlePositions = new List<PosAndRotation>();

    void FixedUpdate()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int particlesAlive = particleSystem.GetParticles(particles);
        if (particlesAlive == 0)
            return;

        particlePositions.Clear();
        for (int i = 0; i < particlesAlive; i++) {
            particlePositions.Add(new PosAndRotation(particles[i].position, Quaternion.Euler(0, 0, -particles[i].rotation)));
        }
    }

    void OnParticleSystemStopped()
    {
        var effectsController = GameObject.Find("+Effects").GetComponent<EffectsController>();
        foreach (var pos in particlePositions) {
            effectsController.SpawnBloodStain(pos.position, pos.rotation);
        }
        Destroy(gameObject);
    }
}
