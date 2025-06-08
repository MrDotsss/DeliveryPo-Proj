using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Singleton manager for handling game settings such as audio, graphics, and controls.
/// Manages volume, fullscreen, resolution index, mouse sensitivity, and invert Y axis.
/// Supports saving and loading settings via PlayerPrefs.
/// </summary>
public class GameSettings : BaseManager<GameSettings>
{
    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;

    // Audio volume levels (0..1)
    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }

    // Graphics settings
    public bool IsFullscreen { get; private set; }
    public int ResolutionIndex { get; private set; }

    // Controls settings
    public float MouseSensitivity { get; private set; } = 0.1f;
    public bool InvertY { get; private set; }

    public event Action OnSettingsChanged;

    public void Start()
    {
        // LoadSettings is currently commented out, enable if needed on start
        // LoadSettings();
    }

    //#region Audio
    // The audio setters are commented out but show how volume levels could be applied to the mixer and saved

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

    /// <summary>
    /// Sets fullscreen mode and saves the setting.
    /// </summary>
    /// <param name="fullscreen">Enable fullscreen or not</param>
    public void SetFullscreen(bool fullscreen)
    {
        IsFullscreen = fullscreen;
        Screen.fullScreen = fullscreen;
        Save();
    }

    #endregion

    #region Controls

    /// <summary>
    /// Sets mouse sensitivity and saves the setting.
    /// </summary>
    public void SetMouseSensitivity(float value)
    {
        MouseSensitivity = value;
        Save();
    }

    /// <summary>
    /// Sets invert Y axis option and saves the setting.
    /// </summary>
    public void SetInvertY(bool invert)
    {
        InvertY = invert;
        Save();
    }

    #endregion

    // The ApplyAllSettings method was commented out — useful if you want to apply all saved settings at once

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

    /// <summary>
    /// Loads saved settings from PlayerPrefs or uses default values.
    /// </summary>
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

    /// <summary>
    /// Saves current settings to PlayerPrefs and triggers OnSettingsChanged event.
    /// </summary>
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
