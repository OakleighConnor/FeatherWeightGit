using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public bool battle;

    [Header("Scripts")]
    PlatformManager pm;

    [Header("Music")]
    public AudioClip title;
    public AudioClip level;
    public AudioClip level2;
    public AudioClip level3;

    [Header("Music Manager")]
    public AudioClip currentTrack;
    public AudioClip targetTrack;
    float targetTime;

    [Header("SFX")]
    // Combat :
    public AudioClip shoot;
    public AudioClip grapple;
    public AudioClip grappleConnect;
    public AudioClip grappleDisconnect;
    public AudioClip explosion;
    public AudioClip woosh;
    public AudioClip enemyDamage;
    public AudioClip heal;
    // UI :
    public AudioClip highlightUI;
    public AudioClip inputUI;

    [Header("Mixers")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    GameObject settingsMenu;

    public AudioTrack track;
    public enum AudioTrack
    {
        title,
        level1,
        level2,
        level3
    }

    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }

        settingsMenu = GameObject.FindGameObjectWithTag("Settings").transform.GetChild(0).gameObject;

        // To make the game play the title theme on launch:

        musicSource.clip = level;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        pm = FindAnyObjectByType<PlatformManager>();

        if (pm != null)
        {
            if (pm.enemiesRemaining > 3)
            {
                targetTrack = level3;
            }
            else if (pm.enemiesRemaining != 0)
            {
                targetTrack = level2;
            }
            else
            {
                targetTrack = level;
            }
        }
        else
        {
            targetTrack = title;
        }

        if(targetTrack != currentTrack)
        {
            // Sets the duration that the clip should play from
            targetTime = musicSource.time;

            // Plays the target track as well as the time that it should play from
            musicSource.clip = targetTrack;
            musicSource.Play();
            musicSource.time = targetTime;
        }

        currentTrack = targetTrack;
    }

    public void RestartMusic()
    {
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlaySFX2()
    {
        SFXSource.PlayOneShot(inputUI);
    }

    public void SetMusicVolume()
    {
        musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSFXVolume()
    {
        musicSlider = GameObject.FindGameObjectWithTag("SFXSlider").GetComponent<Slider>();
        float volume = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetMusicVolume();
        SetSFXVolume();
    }
}
