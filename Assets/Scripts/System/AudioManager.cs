using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [Space]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolume;
    [SerializeField] private string musicVolume;
    [SerializeField] private string soundVolume;

    //Music
    private float disableTimer;
    private DateTime startDate;
    private DateTime currentDate;
    private float seconds;

    [SerializeField] private AudioFiles[] musicFiles;

    [Header("Utiltiy")]
    [SerializeField] public AudioFiles[] utilityFiles;

    [Header("Player")]
    [SerializeField] public AudioFiles[] playerSounds;

    [Header("PlayerHitSounds")]
    [SerializeField] public AudioFiles[] playerHitSounds;

    [Header("Enemy")]
    [SerializeField] public AudioFiles[] enemySounds;

    [Space]
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float randomVolumeMultipler;

    public enum MusicSongs
    {
        Empty,
        TitleScreen,
        ArenaHub,
        Boss
    }
    public enum UtilitySounds
    {
        Empty,
        MenuSelect,
        CurrencyGain,
    }
    public enum PlayerSounds
    {
        Empty,
        PlayerDeath,
    }
    public enum EnemySounds
    {
        Empty
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (PlayerPrefs.GetInt("AudioHasBeenChange") == 0)
        {
            PlayerPrefs.SetFloat("SliderValue" + masterVolume, 0.8f);
            PlayerPrefs.SetFloat(masterVolume, Mathf.Log10(PlayerPrefs.GetFloat("SliderValue" + masterVolume) * 20));
            SetVolume(masterVolume, 0);

            PlayerPrefs.SetFloat("SliderValue" + musicVolume, 0.8f);
            PlayerPrefs.SetFloat(musicVolume, Mathf.Log10(PlayerPrefs.GetFloat("SliderValue" + musicVolume) * 20));
            SetVolume(musicVolume, 0);

            PlayerPrefs.SetFloat("SliderValue" + soundVolume, 2.4f);
            PlayerPrefs.SetFloat(soundVolume, Mathf.Log10(PlayerPrefs.GetFloat("SliderValue" + soundVolume) * 20));
            SetVolume(soundVolume, 0);
        }
        else
        {
            SetVolume(masterVolume, 0);
            SetVolume(musicVolume, 0);
            SetVolume(soundVolume, 10);
        }

    }
    private void SetVolume(string volumename, float maxdb)
    {
        audioMixer.SetFloat(volumename, PlayerPrefs.GetFloat(volumename));
        bool gotvalue = audioMixer.GetFloat(volumename, out float soundvalue);
        if (gotvalue == true)
        {
            if (soundvalue > maxdb)
            {
                audioMixer.SetFloat(volumename, maxdb);
            }
        }
    }
    public void SetSong(int songNumber)
    {
        if (musicSource.clip == musicFiles[songNumber].audioClip) return;

        StopAllCoroutines();
        musicSource.volume = musicFiles[songNumber].volume;
        musicSource.clip = musicFiles[songNumber].audioClip;
        musicSource.Play();
    }

    public void PlayUtilityOneshot(int soundClip)
    {
        if (soundSource != null) soundSource.PlayOneShot(utilityFiles[soundClip].audioClip, utilityFiles[soundClip].volume);
    }
    public void PlayAudioFileOneShot(AudioFiles file)
    {
        if (soundSource != null) soundSource.PlayOneShot(file.audioClip, file.volume);
    }
    public void PlayRandomOneShot(AudioFiles[] files)
    {
        int randomNumber = UnityEngine.Random.Range(0, files.Length);
        //float randomVolume = files[randomNumber].volume * randomVolumeMultipler;

        if (soundSource != null) soundSource.PlayOneShot(files[randomNumber].audioClip, files[randomNumber].volume);
    }

    public void StartMusicFadeOut(int audioFile, bool ignoreSameClip, float fadeOutSpeed, float fadeInSpeed)
    {
        if (ignoreSameClip == false)
        {
            if (musicSource.clip == musicFiles[audioFile].audioClip) return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeOutMusicVolume(audioFile, fadeOutSpeed, fadeInSpeed));
    }
    public IEnumerator FadeOutMusicVolume(int audioFile, float fadeOutSpeed, float fadeInSpeed)
    {
        float duration = fadeOutSpeed;
        float start = musicSource.volume;
        float targetVolume = 0;
        startDate = DateTime.Now;
        disableTimer = 0f;
        while (disableTimer < duration)
        {
            currentDate = DateTime.Now;
            seconds = currentDate.Ticks - startDate.Ticks;
            disableTimer = seconds * 0.0000001f;
            musicSource.volume = Mathf.Lerp(start, targetVolume, disableTimer / duration);
            yield return null;
        }

        musicSource.clip = musicFiles[audioFile].audioClip;
        musicSource.Play();

        if (musicFiles[audioFile].audioClip != null)
        {
            StartCoroutine(FadeInMusicVolume(audioFile, fadeInSpeed, 0));
        }
    }
    public IEnumerator FadeInMusicVolume(int audioFile, float fadeinspeed, float startvolume)
    {
        float duration = fadeinspeed;
        float start = startvolume;
        startDate = DateTime.Now;
        disableTimer = 0f;
        while (disableTimer < duration)
        {
            currentDate = DateTime.Now;
            seconds = currentDate.Ticks - startDate.Ticks;
            disableTimer = seconds * 0.0000001f;
            musicSource.volume = Mathf.Lerp(start, musicFiles[audioFile].volume, disableTimer / duration);
            yield return null;
        }
        yield break;
    }
}
[Serializable]
public struct AudioFiles
{
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume;
}
