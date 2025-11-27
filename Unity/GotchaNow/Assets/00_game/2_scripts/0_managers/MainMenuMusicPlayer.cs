using UnityEngine;
using FMODUnity;

public class MainMenuMusicPlayer : MonoBehaviour
{
    public StudioEventEmitter titleScreenEmitter;
    
    void Start()
    {
        PlayTitleScreenMusic();
    }

    public void PlayTitleScreenMusic()
    {
        Debug.Log("play title screen music");
        
        if (!titleScreenEmitter.IsPlaying())
        {
            titleScreenEmitter.Play();
        }
    }

    public void StopTitleScreenMusic()
    {
        Debug.Log("stop title screen music");
        
        if (titleScreenEmitter.IsPlaying())
        {
            titleScreenEmitter.Stop();
        }
    }

    /* public void SetBGMVolume(float value)
    {
        RuntimeManager.StudioSystem.setParameterByName("bgmVolume", value, false);
    } */
}
