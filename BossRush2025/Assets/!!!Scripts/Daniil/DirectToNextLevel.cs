using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DirectToNextLevel : MonoBehaviour
{
    private List<ParticleSystem> _allDirectParticles = new List<ParticleSystem>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.TryGetComponent<ParticleSystem>(out ParticleSystem currentParticle);
            if (currentParticle != null)
            {
                _allDirectParticles.Add(currentParticle);
            }
        }
        GameObject.FindAnyObjectByType<GameManager>().BossDefeat += () =>
        {
            foreach (ParticleSystem currentParticle in _allDirectParticles)
            {
                currentParticle.Play();
            }
        };
    }
}
