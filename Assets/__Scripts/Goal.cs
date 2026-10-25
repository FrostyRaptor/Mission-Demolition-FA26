using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
  [Header("Audio Settings")]
  public AudioClip soundToPlay;

  [Header("Dynamic")]
  public ParticleSystem confettiParticles;

  static public bool goalMet = false;
  private bool hasTriggered = false;
  private AudioSource cleanAudioSource;

  private void Awake()
  {
    if (confettiParticles == null)
    {
      confettiParticles = GetComponentInChildren<ParticleSystem>(true);
    }
  }

  private void Start()
  {
    if (MissionDemolition.Instance != null)
    {
      MissionDemolition.Instance.RegisterCurrentGoal(this);
    }

    cleanAudioSource = gameObject.AddComponent<AudioSource>();

    cleanAudioSource.enabled = true;
    cleanAudioSource.playOnAwake = false;
  }

  private void OnTriggerEnter(Collider other)
  {
    Projectile proj = other.GetComponent<Projectile>();
    if (proj != null)
    {
      goalMet = true;

      Material material = GetComponent<Renderer>().material;
      Color c = material.color;
      c.a = 0.75f;
      material.color = c;

      if (MissionDemolition.Instance != null)
      {
        MissionDemolition.Instance.RegisterCurrentGoal(this);
      }
    }
  }

  public void TriggerConfetti()
  {
    if (hasTriggered || confettiParticles == null) return;

    hasTriggered = true;
    confettiParticles.gameObject.SetActive(true);
    confettiParticles.Play(true);

    PlayGoalSound();
  }

  public void ResetConfetti()
  {
    if (confettiParticles != null)
    {
      confettiParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      confettiParticles.gameObject.SetActive(false);
    }

    hasTriggered = false;
  }

  void PlayGoalSound()
  {
    if (soundToPlay == null) return;

    cleanAudioSource.PlayOneShot(soundToPlay);
  }
}
