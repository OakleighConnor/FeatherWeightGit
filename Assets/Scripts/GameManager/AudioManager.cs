using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public AudioClip title;
    public AudioClip level;

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
