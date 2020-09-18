using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource musicSrc, effectsSrc;

    [SerializeField]
    private Slider musicSlider, effectsSlider;

    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();


    // Start is called before the first frame update
    void Start()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds") as AudioClip[];
        foreach (AudioClip clip in clips)
        {
            //Debug.Log(clip.name);
            audioClips.Add(clip.name, clip);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayEffect(string name)
    {
        effectsSrc.PlayOneShot(audioClips[name], 1);
    }

    public void UpdateMusicVolume()
    {
        musicSrc.volume = musicSlider.value;
    }
    public void UpdateEffectsVolume()
    {
        effectsSrc.volume = effectsSlider.value;
    }

    
}
