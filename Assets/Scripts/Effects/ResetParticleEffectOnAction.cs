using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetParticleEffectOnAction : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    public void ResetParticleSystem()
    {
        _particleSystem.Clear();
        _particleSystem.Play();
    }
}
