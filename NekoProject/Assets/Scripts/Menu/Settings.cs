////////////////////////////////////////////////////////////////////////////////////////////////////////
//Author : Ruben Vidorreta
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider sfxSlider, musicSlider;
    // Start is called before the first frame update
    void Start()
    {
        sfxSlider.value = AudioManager.Instance.SfxVolume;
        musicSlider.value = AudioManager.Instance.MusicVolume;
    }


    public void SFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }

    public void MusicVolume(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
    }

    public void ToggleMute()
    {
        AudioManager.Instance.PlaySound("Button");
        AudioManager.Instance.ToggleMute();
    }
}
