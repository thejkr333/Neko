using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume = 1f;

    [Range(-3f, 3f)]
    public float Pitch = 1f;

    public bool Loop = false;
}

[System.Serializable]
public class MusicTrack
{
    public string Name;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public List<Sound> Sounds = new();
    public List<MusicTrack> Music = new();
    List<MusicTrack> GameMusicTrack = new();

    private Dictionary<string, AudioSource> soundSources = new();
    private Dictionary<string, AudioClip> musicClips = new();
    private AudioSource musicSource;

    [SerializeField] private float fadeDuration = 1f;

    [Range(0f, 1f)]
    public float MusicVolume = 1f;

    [Range(0f, 1f)]
    public float SfxVolume = 1f;

    private bool isMuted, isPaused;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound sound in Sounds)
        {
            AudioSource _source = gameObject.AddComponent<AudioSource>();
            _source.clip = sound.Clip;
            _source.volume = sound.Volume;
            _source.pitch = sound.Pitch;
            _source.playOnAwake = false;
            _source.loop = sound.Loop;
            soundSources[sound.Name] = _source;
        }

        foreach (MusicTrack music in Music)
        {
            if (music.Name.StartsWith("Game")) GameMusicTrack.Add(music);
            musicClips[music.Name] = music.Clip;
        }

        // Initialize musicSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = 1f;
        musicSource.pitch = 1f;

        if (PlayerPrefs.HasKey("SFXVolume")) SfxVolume = PlayerPrefs.GetFloat("SFXVolume"); else SfxVolume = 1f;
        if (PlayerPrefs.HasKey("MusicVolume")) MusicVolume = PlayerPrefs.GetFloat("MusicVolume"); else MusicVolume = 1f;
    }

    #region SFX
    public void PlaySound(string name, float pitch = 1)
    {
        if (isMuted) return;

        if (soundSources.ContainsKey(name))
        {
            AudioSource _source = soundSources[name];

            if (_source.loop && _source.isPlaying) return;

            _source.volume = SfxVolume;
            _source.pitch = pitch;
            _source.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: Sound not found - " + name);
        }
    }

    public void StopSound(string name)
    {
        if (isMuted) return;

        if (soundSources.ContainsKey(name))
        {
            AudioSource _source = soundSources[name];
            _source.Stop();

        }
        else
        {
            Debug.LogWarning("AudioManager: Sound not found - " + name);
        }
    }
    #endregion

    #region Music
    public void PlayMusic(string name)
    {
        if (isMuted) return;

        if (musicClips.ContainsKey(name))
        {
            if (musicSource.clip == null)
            {
                musicSource.clip = musicClips[name];
                StartCoroutine(FadeInAudio(musicSource, MusicVolume));
                StartCoroutine(PlayNextSong(name));
            }
            else if (musicSource.clip != musicClips[name])
            {
                StartCoroutine(FadeOutMusic(() =>
                {
                    musicSource.clip = musicClips[name];
                    StartCoroutine(PlayNextSong(name));
                    StartCoroutine(FadeInAudio(musicSource, MusicVolume));
                }));
            }
        }
        else
        {
            Debug.LogWarning("AudioManager: Music not found - " + name);
        }
    }

    private IEnumerator FadeOutMusic(System.Action callback = null)
    {
        float _startVolume = musicSource.volume;

        while (musicSource.volume > 0)
        {
            musicSource.volume -= _startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = _startVolume;

        if (callback != null) callback();
    }

    IEnumerator FadeInAudio(AudioSource source, float targetVolume)
    {
        // Fade in new music
        source.volume = 0f;
        source.Play();
        while (Mathf.Abs(source.volume - targetVolume) > 0.01f)
        {
            source.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        source.volume = targetVolume;
    }

    IEnumerator PlayNextSong(string currentSong)
    {
        float timeForNextSong = musicClips[currentSong].length - fadeDuration * 2f;

        yield return new WaitForSeconds(timeForNextSong);

        string _newSong;
        if (GameManager.Instance.GetCurrentScene() != "BosqueTurquesa") 
        { 
            _newSong = currentSong;
        }
        else
        {
            do
            {
                _newSong = GameMusicTrack[UnityEngine.Random.Range(1, GameMusicTrack.Count)].Name;
            }
            while (_newSong == currentSong);
        }

        PlayMusic(_newSong);
    }
    #endregion

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;

        if (musicSource != null)
        {
            musicSource.volume = volume;
        }

        // Save the music volume to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SfxVolume = volume;

        // Update the volume of all existing sound sources
        foreach (AudioSource source in soundSources.Values)
        {
            source.volume = volume;
        }

        // Save the SFX volume to PlayerPrefs
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            musicSource.Pause();

            foreach (KeyValuePair<string, AudioSource> sound in soundSources)
            {
                sound.Value.Pause();
            }
        }
        else
        {
            musicSource.UnPause();

            foreach (KeyValuePair<string, AudioSource> sound in soundSources)
            {
                if (sound.Value.isPlaying)
                {
                    sound.Value.UnPause();
                }
            }
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            musicSource.Stop();

            foreach (KeyValuePair<string, AudioSource> sound in soundSources)
            {
                sound.Value.Stop();
            }
        }
        else
        {
            musicSource.Play();

            foreach (KeyValuePair<string, AudioSource> sound in soundSources)
            {
                if (sound.Value.isPlaying)
                {
                    sound.Value.Play();
                }
            }
        }
    }
}
