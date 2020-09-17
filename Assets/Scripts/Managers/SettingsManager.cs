using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Settings Component References
    
    [SerializeField]
    private Slider volumeSliderMusic, volumeSliderEffects;
    [SerializeField]
    public Dropdown resolution;
    [SerializeField]
    public Toggle fullScreen;
    #endregion

    #region Player Pref Key Constants

    private const string MUSIC_VOLUME_PREF = "music-volume";
    private const string EFFECTS_VOLUME_PREF = "effects-volume";
    private const string RESOLUTION_PREF = "resolution";
    private const string FULLSCREEN_PREF = "fullscreen";

    #endregion



    // Start is called before the first frame update
    void Start()
    {
        var resolutions = resolution.options.Select(option => option.text).ToList();
        volumeSliderMusic.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF, 1);
        volumeSliderEffects.value = PlayerPrefs.GetFloat(EFFECTS_VOLUME_PREF, 1);
        fullScreen.isOn = GetBoolPref(FULLSCREEN_PREF, true);
        resolution.value = resolutions.IndexOf(PlayerPrefs.GetString(RESOLUTION_PREF, "1280x720"));
    }

    public void FullScreenToggle()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void ResolutionChange()
    {
        string[] res = resolution.options[resolution.value].text.Split('x');
        Screen.SetResolution(Convert.ToInt32(res[0]), Convert.ToInt32(res[1]), fullScreen.isOn);
    }

    #region Graphics

    public void OnToggleFullScreen(bool state)
    {
        if (fullScreen != null)
        {
            SetPref(FULLSCREEN_PREF, fullScreen.isOn);
        }
        else
        {
            SetPref(FULLSCREEN_PREF, state);
        }
    }

    public void OnChangeResolution(string value)
    {
        if (resolution.value >= 0)
        {
            SetPref(RESOLUTION_PREF, resolution.options[resolution.value].text);
        }
        else
        {
            SetPref(RESOLUTION_PREF, value);
        }
    }

    #endregion

    #region VolumePrefs

    public void OnChangeMusicVolume(Single value)
    {
        if (volumeSliderMusic.value >= 0f)
        {
            SetPref(MUSIC_VOLUME_PREF, volumeSliderMusic.value);
        }
        else
        {
            SetPref(MUSIC_VOLUME_PREF, value);
        }
    }

    public void OnChangeEffectsVolume(Single value)
    {
        if (volumeSliderMusic.value >= 0f)
        {
            SetPref(EFFECTS_VOLUME_PREF, volumeSliderEffects.value);
        }
        else
        {
            SetPref(EFFECTS_VOLUME_PREF, value);
        }
    }

    #endregion

    #region Sets and Gets Prefs
    private void SetPref(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    private void SetPref(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    private void SetPref(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    private void SetPref(string key, bool value)
    {
        PlayerPrefs.SetInt(key, Convert.ToInt32(value));
    }

    private bool GetBoolPref(string key, bool defaultValue = true)
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt(key, Convert.ToInt32(defaultValue)));
    }
    #endregion
}
