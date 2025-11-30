using UnityEngine;
using FMODUnity;

namespace GotchaNow
{
	[CreateAssetMenu(menuName = "ScriptableObjects/VolumeControl", order = 100)]
	public class VolumeControl : ScriptableObject
	{
		[SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
		[SerializeField, Range(0f, 1f)] private float musicVolume = 1f;

		public float SFXVolume
		{
			get => sfxVolume;
			set
            {
				sfxVolume = Mathf.Clamp01(value);
				RuntimeManager.StudioSystem.setParameterByName("sfxVolume", value, false);
            } 
		}

		public float MusicVolume
		{
			get => musicVolume;
            set
            {
				musicVolume = Mathf.Clamp01(value);
				RuntimeManager.StudioSystem.setParameterByName("bgmVolume", value, false);
            } 
		}


	} 
}
