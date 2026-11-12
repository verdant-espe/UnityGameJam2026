
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Voice Clips")]
    [SerializeField] private AudioClip[] vocalizations;
    [SerializeField] private AudioClip[] damageVocalizations;
    [SerializeField] private AudioClip deathSound;

    [Header("Ambient Timing")]
    [SerializeField] private float minAmbientDelay = 5f;
    [SerializeField] private float maxAmbientDelay = 12f;

    [Header("Vocalization Chances")]
    [SerializeField, Range(0f, 1f)]
    private float jumpChance = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float damageChance = 0.4f;

    [Header("Double Vocalizations")]
    [SerializeField, Range(0f, 1f)]
    private float doubleSoundChance = 0.25f;

    [SerializeField] private float secondSoundDelay = 0.4f;

    private Coroutine ambientCoroutine;
    private bool isDead;

    private void Awake()
    {
        if (voiceSource == null)
        {
            voiceSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        isDead = false;
        ambientCoroutine = StartCoroutine(AmbientSounds());
    }

    private void OnDisable()
    {
        if (ambientCoroutine != null)
        {
            StopCoroutine(ambientCoroutine);
            ambientCoroutine = null;
        }
    }

    private IEnumerator AmbientSounds()
    {
        while (!isDead)
        {
            float delay = Random.Range(
                minAmbientDelay,
                maxAmbientDelay
            );

            yield return new WaitForSeconds(delay);

            if (isDead)
            {
                yield break;
            }

            // Do not interrupt an existing vocalization.
            if (!voiceSource.isPlaying)
            {
                PlayRandomClip(vocalizations);

                // Occasionally play a second sound.
                if (Random.value < doubleSoundChance)
                {
                    yield return new WaitForSeconds(
                        secondSoundDelay
                    );

                    if (!isDead && !voiceSource.isPlaying)
                    {
                        PlayRandomClip(vocalizations);
                    }
                }
            }
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (voiceSource == null ||
            clips == null ||
            clips.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, clips.Length);

        if (clips[index] == null)
        {
            return;
        }

        // Interrupt the previous enemy vocalization.
        voiceSource.Stop();
        voiceSource.clip = clips[index];
        voiceSource.Play();
    }

    public void PlayJump()
    {
        if (!isDead && Random.value < jumpChance)
        {
            PlayRandomClip(vocalizations);
        }
    }

    public void PlayLunge()
    {
        if (!isDead)
        {
            PlayRandomClip(vocalizations);
        }
    }

    public void PlayDamage()
    {
        if (isDead || Random.value >= damageChance)
        {
            return;
        }

        // Use specialized damage clips when available.
        if (damageVocalizations != null &&
            damageVocalizations.Length > 0)
        {
            PlayRandomClip(damageVocalizations);
        }
        else
        {
            PlayRandomClip(vocalizations);
        }
    }

    public void PlayDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (voiceSource != null)
        {
            voiceSource.Stop();
        }

        // Play through SoundManager so the death
        // sound survives destruction of this enemy.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(deathSound);
        }
    }
}
