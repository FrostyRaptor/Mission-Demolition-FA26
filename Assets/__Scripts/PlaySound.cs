using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
  [Header("Audio Settings")]
  public AudioClip soundToPlay;

  [Header("Inscribed")]
  public Slingshot slingshotGO;

  private AudioSource cleanAudioSource;

  void Start()
  {
    cleanAudioSource = gameObject.AddComponent<AudioSource>();

    cleanAudioSource.enabled = true;
    cleanAudioSource.playOnAwake = false;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && !slingshotGO.aimingMode)
    {
      if (soundToPlay == null)
      {
        Debug.LogError($"[Audio Error] You forgot to assign the Sound To Play file in the Inspector on {gameObject.name}!");
        return;
      }

      cleanAudioSource.PlayOneShot(soundToPlay);
    }
  }
}
