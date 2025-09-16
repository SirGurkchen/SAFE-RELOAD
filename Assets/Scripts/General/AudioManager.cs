using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class every sound has.
/// Sets SoundType and the audio clip.
/// </summary>
[System.Serializable]
public class Sound
{
    public AudioManager.SoundType type;
    public AudioClip clip;
}

/// <summary>
/// Handles the Audio of the game.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioSource walkingSource;
    [SerializeField] private AudioSource enemyDieSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<Sound> sounds;

    /// <summary>
    /// The currently available sound types.
    /// </summary>
    public enum SoundType
    {
        Reload,
        ReloadFinished,
        Shoot,
        GunEmpty,
        Walking,
        EnemyDeath,
        Music,
        Click
    }

    private Dictionary<SoundType, AudioClip> soundDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            BuildDictionary();
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (musicSource != null && soundDictionary.TryGetValue(SoundType.Music, out AudioClip musicClip))
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.Log("Music not loaded!");
        }
    }
    
    /// <summary>
    /// Plays a given sound selected by the given sound type.
    /// The sound will be played with the generic sound source.
    /// </summary>
    /// <param name="type">The type of the sound played</param>
    public void PlaySound(SoundType type)
    {
        if (soundDictionary.TryGetValue(type, out AudioClip clip))
        {
            soundSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("Sound not found!");
        }
    }

    /// <summary>
    /// Checks if the player is walking by the given parameter.
    /// If the player is moving, plays the walking audio.
    /// If the player is not moving, stops the walking audio.
    /// Audio is played with the walkingSource.
    /// </summary>
    /// <param name="isMoving">Boolean if the player is moving</param>
    public void PlayWalkAudio(bool isMoving)
    {
        if (isMoving && !walkingSource.isPlaying)
        {
            walkingSource.clip = soundDictionary[SoundType.Walking];
            walkingSource.loop = true;
            walkingSource.Play();
        }
        else if (!isMoving && walkingSource.isPlaying)
        {
            walkingSource.Stop();
        }
    }

    public void PlayEnemyDieAudio()
    {
        if (enemyDieSource != null)
        {
            enemyDieSource.PlayOneShot(soundDictionary[SoundType.EnemyDeath]);
        }
    }

    private void BuildDictionary()
    {
        soundDictionary = new Dictionary<SoundType, AudioClip>();
        foreach (Sound s in sounds)
        {
            soundDictionary[s.type] = s.clip;
        }
    }

    public void DisableAudio()
    {
        soundSource.Stop();
        walkingSource.Stop();
        enemyDieSource.Stop();
    }

    public void PlayerDisableEnableMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else
        {
            musicSource.Play();
        }
    }
}