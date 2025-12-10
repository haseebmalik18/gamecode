using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    
    AudioSource audioSource;
    public AudioClip music;
    
    bool isMuted = false;
    float savedVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupAudio()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        audioSource.volume = isMuted ? 0f : savedVolume;
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (music != null && !audioSource.isPlaying)
        {
            audioSource.clip = music;
            audioSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        savedVolume = volume;
        if (!isMuted && audioSource != null)
            audioSource.volume = volume;
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        if (isMuted)
            audioSource.volume = 0f;
        else
            audioSource.volume = savedVolume;
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
