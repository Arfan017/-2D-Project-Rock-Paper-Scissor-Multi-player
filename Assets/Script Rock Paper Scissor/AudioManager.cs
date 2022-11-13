using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public AudioMixer mixer;
    public Slider SFXSlider;
    public Slider BGMSlider;
    void Start()
    {
        float db;
        if (mixer.GetFloat("SFX_VOL", out db))
        {
            SFXSlider.value = (db + 80)/80;
        }

        if (mixer.GetFloat("SFX_VOL", out db))
        {
            BGMSlider.value = (db + 80)/80;
        }
    }

    public void SFXVolume(float value){

        value = value * 80 - 80;
        
        mixer.SetFloat("SFX_VOL", value); 
    }
    public void BGMVolume(float value){

        value = value * 80 - 80;
        
        mixer.SetFloat("BGM_VOL", value); 
    }
}