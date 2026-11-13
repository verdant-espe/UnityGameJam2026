
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Scene Music")]
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;

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
        // Only one SoundManager can exist.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep the manager and its AudioSources between scenes.
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Start the correct music when the game first launches.
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop sound effects left over from the previous scene.
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }

        // Switch to the music assigned to the new scene.
        PlayMusicForScene(scene.name);
    }


    private void PlayMusicForScene(string sceneName)
    {
        if (sceneName == "TitleScene")
        {
            PlayMusic(titleMusic);
        }
        else if (sceneName == "GameScene")
        {
            PlayMusic(gameplayMusic);
        }
        else if (sceneName == "GameOverScene")
        {
            PlayMusic(gameOverMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        // Don't restart music if the correct song is already playing.
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

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

    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0)
            return;

        int randomIndex = Random.Range(0, clips.Length);
        PlaySFX(clips[randomIndex], volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

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
        PlayRandomSFX(clothImpacts, 0.8f);
    }

    public void PlayRabbitDeath()
    {
        PlaySFX(rabbitDeath);
    }
}
