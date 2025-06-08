using System;
using UnityEngine;
using UnityEngine.Audio;

public class GameSettings : BaseManager<GameSettings>
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    public bool IsFullscreen { get; private set; }
    public int ResolutionIndex { get; private set; }

    public float MouseSensitivity { get; private set; } = 0.1f;
    public bool InvertY { get; private set; }

    public event Action OnSettingsChanged;

    public void Start()
    {
        //LoadSettings();
    }

    //#region Audio
    //public void SetMasterVolume(float value)
    //{
    //    MasterVolume = value;
    //    audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    //    Save();
    //}

    //public void SetMusicVolume(float value)
    //{
    //    MusicVolume = value;
    //    audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
    //    Save();
    //}

    //public void SetSFXVolume(float value)
    //{
    //    SFXVolume = value;
    //    audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
    //    Save();
    //}
    //#endregion

    #region Graphics

    public void SetFullscreen(bool fullscreen)
    {
        IsFullscreen = fullscreen;
        Screen.fullScreen = fullscreen;
        Save();
    }
    #endregion

    #region Controls
    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        Save();
    }

    public void SetInvertY(bool invert)
    {
        InvertY = invert;
        Save();
    }
    #endregion

    //private void ApplyAllSettings()
    //{
    //    SetMasterVolume(MasterVolume);
    //    SetMusicVolume(MusicVolume);
    //    SetSFXVolume(SFXVolume);
    //    SetResolution(ResolutionIndex);
    //    SetFullscreen(IsFullscreen);
    //    SetMouseSensitivity(MouseSensitivity);
    //    SetInvertY(InvertY);
    //}

    private void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        IsFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
        MouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.1f);
        InvertY = PlayerPrefs.GetInt("InvertY", 0) == 1;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.SetInt("Fullscreen", IsFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", ResolutionIndex);
        PlayerPrefs.SetFloat("Sensitivity", MouseSensitivity);
        PlayerPrefs.SetInt("InvertY", InvertY ? 1 : 0);
        PlayerPrefs.Save();

        OnSettingsChanged?.Invoke();
    }


}
