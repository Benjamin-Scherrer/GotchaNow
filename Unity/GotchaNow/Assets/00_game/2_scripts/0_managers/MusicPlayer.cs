using UnityEngine;
using FMODUnity;
using System.Drawing;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;
    public StudioEventEmitter intermissionEmitter;
    public StudioEventEmitter battleKatoroEmitter;
    public StudioEventEmitter battleAyaEmitter;
    public 

    void Awake()
    {
        instance = this;    
    }

    public void PlayIntermissionMusic()
    {
        Debug.Log("play intermission music");
        
        if (battleKatoroEmitter.IsPlaying())
        {
            battleKatoroEmitter.Stop();
        }
        if (battleAyaEmitter.IsPlaying())
        {
            battleAyaEmitter.Stop();
        }

        if (!intermissionEmitter.IsPlaying())
        {
            intermissionEmitter.Play();
        }
    }
    
    public void PlayKatoroMusic()
    {
        Debug.Log("play katoro music");
        
        if (intermissionEmitter.IsPlaying())
        {
            intermissionEmitter.Stop();
        }
        if (battleAyaEmitter.IsPlaying())
        {
            battleAyaEmitter.Stop();
        }

        if (!battleKatoroEmitter.IsPlaying())
        {
            battleKatoroEmitter.Play();
        }
    }

    public void PlayAyaMusic()
    {
        Debug.Log("play aya music");
        
        if (intermissionEmitter.IsPlaying())
        {
            intermissionEmitter.Stop();
        }
        if (battleKatoroEmitter.IsPlaying())
        {
            battleKatoroEmitter.Stop();
        }

        if (!battleAyaEmitter.IsPlaying())
        {
            battleAyaEmitter.Play();
        }
    }

    public void SetLowPassFilter(float value)
    {
        RuntimeManager.StudioSystem.setParameterByName("lowPassFilter", value, false);
    }

    public void SetBGMVolume(float value)
    {
        RuntimeManager.StudioSystem.setParameterByName("bgmVolume", value, false);
    }
}
