using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GauntletWeapon))]
[RequireComponent(typeof(AudioSource))]
public class GauntletAudioController : MonoBehaviour
{
    private GauntletWeapon gauntlet;
    private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;

    private void Awake()
    {
        gauntlet = GetComponent<GauntletWeapon>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shootClip;
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        gauntlet.OnGauntletShot += PlayShootSound;
    }

    private void OnDisable()
    {

        gauntlet.OnGauntletShot -= PlayShootSound;
    }

    private void PlayShootSound()
    {
        audioSource.pitch = Random.Range(.9f, 1.1f);
        audioSource.Play();
    }
}
