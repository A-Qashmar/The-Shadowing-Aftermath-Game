using UnityEngine;

public class PlayerAbilityAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip abilitySound;

    public void PlayAbilitySound()
    {
        if (audioSource != null && abilitySound != null)
            audioSource.PlayOneShot(abilitySound);
    }
}
