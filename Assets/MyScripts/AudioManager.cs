using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource backgroundMusicSource;
    public AudioSource soundEffectsSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 배경음 재생
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.loop = true; // 배경음을 루프로 설정
            backgroundMusicSource.Play();
        }
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        Debug.Log("Setting background music volume to: " + volume);
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume;
            if (volume == 0)
            {
                backgroundMusicSource.Pause();
            }
            else
            {
                if (!backgroundMusicSource.isPlaying)
                {
                    backgroundMusicSource.UnPause();
                }
            }
        }
    }

    public void SetSoundEffectsVolume(float volume)
    {
        Debug.Log("Setting sound effects volume to: " + volume);
        if (soundEffectsSource != null)
        {
            soundEffectsSource.volume = volume;
            if (volume == 0)
            {
                soundEffectsSource.Pause();
            }
            else
            {
                if (!soundEffectsSource.isPlaying)
                {
                    soundEffectsSource.UnPause();
                }
            }
        }
    }

    public void MuteAllAudio(bool mute)
    {
        AudioListener.volume = mute ? 0 : 1;
    }
}
