
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 0.7f;

    [Header("Sound Effects")]
    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 1f;

    [Header("Rabbit - Footsteps")]
    [SerializeField] private AudioClip[] rabbitFootsteps;

    [Header("Rabbit - Jump")]
    [SerializeField] private AudioClip[] rabbitJump;

    [Header("Rabbit - Double Jump")]
    [SerializeField] private AudioClip[] rabbitDoubleJump;

    [Header("Rabbit - Gunshot")]
    [SerializeField] private AudioClip[] rabbitGunshot;

    [Header("Rabbit - Damage")]
    [SerializeField] private AudioClip[] rabbitDamage;

    [SerializeField] private AudioClip rabbitDeath;

    [Header("Damage Impacts")]
    [SerializeField] private AudioClip[] clothImpacts;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Plays one randomly selected clip from an array.
    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        PlaySFX(clips[randomIndex], volume);
    }

    // Plays an individual sound effect.
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    // Rabbit sound methods.
    public void PlayRabbitFootstep()
    {
        PlayRandomSFX(rabbitFootsteps);
    }

    public void PlayRabbitJump()
    {
        PlayRandomSFX(rabbitJump);
    }

    public void PlayRabbitDoubleJump()
    {
        PlayRandomSFX(rabbitDoubleJump);
    }

    public void PlayRabbitGunshot()
    {
        PlayRandomSFX(rabbitGunshot);
    }

    public void PlayRabbitDamage()
    {
        PlayRandomSFX(rabbitDamage);
    }

    public void PlayClothImpact()
    {
        // Randomize the impact sample.
        PlayRandomSFX(clothImpacts, 0.8f);
    }

    public void PlayRabbitDeath()
    {
        PlaySFX(rabbitDeath);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
        {
            return;
        }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}
