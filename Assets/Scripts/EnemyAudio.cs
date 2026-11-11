
using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Voice Clips")]
    [SerializeField] private AudioClip[] vocalizations;
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

            PlayRandomVoice();

            // Occasionally play a second monkey sound.
            if (Random.value < doubleSoundChance)
            {
                yield return new WaitForSeconds(secondSoundDelay);

                if (!isDead)
                {
                    PlayRandomVoice();
                }
            }
        }
    }

    private void PlayRandomVoice()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        if (vocalizations == null ||
            vocalizations.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, vocalizations.Length);

        SoundManager.Instance.PlaySFX(
            vocalizations[index]
        );
    }

    public void PlayJump()
    {
        if (!isDead && Random.value < jumpChance)
        {
            PlayRandomVoice();
        }
    }

    public void PlayLunge()
    {
        if (!isDead)
        {
            PlayRandomVoice();
        }
    }

    public void PlayDamage()
    {
        if (!isDead && Random.value < damageChance)
        {
            PlayRandomVoice();
        }
    }

    public void PlayDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(deathSound);
        }
    }
}
